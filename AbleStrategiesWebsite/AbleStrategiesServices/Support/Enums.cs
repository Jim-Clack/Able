using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public enum PurchaseAuthority
    {
        Unknown = 0,
        NoCharge = 1,
        PayPalStd = 2,
    }

    public enum InteractivityClient
    {
        Unknown = 0,
        PhoneCall = 1,
        OnlineChat = 2,
        Email = 3,
        InPerson = 4,
        UserAlert = 7,
        RegistrationWs = 8,
        ActivationWs = 9,
        PollWs = 10,
        OtherWs = 20,
    }

}
