using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    /// <summary>
    /// For tracking callers in order to prevent DDS attacks.
    /// </summary>
    class AntiDds
    {
        /// <summary>
        /// 20,000 = about 25 hits in latency period indicates attack
        /// </summary>
        private long DangerAttackLevel = 20000L;

        /// <summary>
        /// 10,000 = 10 seconds of latency, reduces danger level
        /// </summary>
        private long latencyMillis = 10000;

        /// <summary>
        /// How long must it me latent before it is considered to be stale?
        /// </summary>
        private long staleMillis = 100000;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="ipAddress"></param>
        public AntiDds(string ipAddress)
        {
            this.ipAddress = ipAddress;
        }

        /// <summary>
        /// Handle a call
        /// </summary>
        /// <returns>true if being attacked</returns>
        public bool IsAttacking
        {
            get
            {
                long numMillis = MsSinceLastAccess;
                if (numMillis > latencyMillis) // idle, drop to half of the prior danger level
                {
                    dangerLevel = dangerLevel / 2L;
                }
                else // for time since last hit of 10ms add 100; for 10sec add 1, etc.
                {
                    dangerLevel += (long)Math.Sqrt(Math.Max(1, latencyMillis - numMillis));
                }
                return dangerLevel >= DangerAttackLevel;
            }
        }

        /// <summary>
        /// How long since last hit?
        /// </summary>
        public long MsSinceLastAccess
        {
            get
            {
                return (long)Math.Abs((DateTime.Now - lastAccess).Duration().TotalMilliseconds);
            }
        }

        /// <summary>
        /// Has this host caller been latent a long time?
        /// </summary>
        public bool IsStale
        {
            get
            {
                return MsSinceLastAccess > staleMillis;
            }
        }

        /// <summary>
        /// Host caller's ip
        /// </summary>
        public string ipAddress = null;

        /// <summary>
        /// When were we last call by that host?
        /// </summary>
        public DateTime lastAccess = DateTime.Now;

        /// <summary>
        /// Track danger level for DDS attacks
        /// </summary>
        public long dangerLevel = 0L;
    }

    /// <summary>
    /// Singleton that filters web service calls for DDS and permission. 
    /// </summary>
    public class ClientCallFilter
    {

        /// <summary>
        /// Singleton instance;
        /// </summary>
        private static ClientCallFilter instance = null;

        /// <summary>
        /// Singleton instance;
        /// </summary>
        public static ClientCallFilter Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ClientCallFilter();
                }
                return instance;
            }
        }

        /// <summary>
        /// For dealing with AntiDdsIpAddresses concurrently.
        /// </summary>
        private static Mutex AntiDdsMutex = new Mutex();

        /// <summary>
        /// For tracking callers.
        /// </summary>
        private static SortedList<string, AntiDds> IpClientsList = new SortedList<string, AntiDds>();

        /// <summary>
        /// Each API must call this first.
        /// </summary>
        /// <param name="remoteIp">HttpContext.Connection.RemoteIpAddress</param>
        /// <param name="restricted">true if this API is restricted to non-users, AbleStrategies only.</param>
        /// <param name="ipAddess">will be populated with man-readable host id</param>
        /// <returns>true if okay, false to abort call</returns>
        public bool Validate(System.Net.IPAddress remoteIp, bool restricted, out string ipAddess)
        {
            bool okay = true;
            AntiDdsMutex.WaitOne();
            ipAddess = remoteIp.ToString();
            // Check for DDS attack
            AntiDds antiDds = null;
            if(!IpClientsList.TryGetValue(ipAddess, out antiDds))
            {
                antiDds = new AntiDds(ipAddess);
                IpClientsList.Add(ipAddess, antiDds);
            }
            try
            {
                if (antiDds.IsAttacking)
                {
                    Logger.Warn(ipAddess, "Apparent DDS attack is underway, lastAccess=" + 
                        antiDds.lastAccess + " dangerLevel=" + antiDds.dangerLevel);
                    // Future - do something about it! Right now we'll just throttle it with the antiDdsMutex:
                    Thread.Sleep(1000);
                    okay = false;
                }
                // delete random hosts from DdsTracker if they become stale
                bool tryAgain = IpClientsList.Count > 1;
                while (tryAgain)
                {
                    int index = new Random((int)DateTime.Now.Millisecond).Next(IpClientsList.Count);
                    tryAgain = IpClientsList.Values[index].IsStale;
                    if (tryAgain)
                    {
                        IpClientsList.RemoveAt(index);
                    }
                }
            }
            finally
            {
                AntiDdsMutex.ReleaseMutex();
            }
            // Check for permission
            if (restricted && !Configuration.Instance.IsHyperUser(remoteIp))
            {
                Logger.Warn(remoteIp.ToString(), "Attempted unauthorized access, host permission not granted");
                okay = false;
            }
            return okay;
        }

    }
}
