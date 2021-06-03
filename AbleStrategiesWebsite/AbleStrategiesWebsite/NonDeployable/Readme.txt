Setup
  Install Visual Studio 2019
  Open the solution
  If not already done, tell the IONOS installer to install .NET core: 
    Add the following line to AbleStrategiesWebsite.csproj <PropertyGoup> element:
      <PublishWithAspNetCoreTargetManifest>false</PublishWithAspNetCoreTargetManifest>
  Install WinSCP on your computer

Caveats
  

Connection to IONOS Server

  SessionOptions sessionOptions = new SessionOptions
  {
    Protocol = Protocol.Sftp,
    HostName = "access872699540.webspace-data.io",
    UserName = "u104847980",
    Password = "XXXXXXX.XX",
    SshHostKeyFingerprint = "ssh-ed25519 255 1gx2w8Rtv3wCgi7Jh8myf/KVd72cRQbow03UP8P095Q=",
  };

  using (Session session = new Session())
  {
    session.Open(sessionOptions);
    // etc.
  }