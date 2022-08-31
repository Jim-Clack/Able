using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// These enums needd to be treated as ints over the web services so as not to expose 
/// internal values. Also note that these enums do not belong in the WsApi subdirectory!
/// </summary>
namespace AbleLicensing
{

    /// <summary>
    /// Delimiter in LicenseCode corresponds to user level in Configuration.
    /// </summary>
    public enum UserLevelPunct
    {
        Evaluation = 0,
        Unknown = '~',
        Deactivated = (int)'–', // note: en-dash, not a hyphen
        Standard = (int)'-',
        ProCPA = (int)'&',
        SuperUser = (int)'@',
    }

    /// <summary>
    /// Web service API states (both requests and responses, not really a state any more)
    /// </summary>
    public enum ApiState
    {
        Unknown = 0,
        // Request that passes minimal info and does not update DB
        LookupLicense = 2,           // Find my info by license code
        // Requests that expect info, as is known, to be populated
        RegisterLicense = 5,         // May alter/update licenseCode in returned UserInfo
        UpdateInfo = 6,              // Change addr, phone, email, etc
        ChangeFeature = 7,           // Change the feature mask
        ChangeLevel = 8,             // Change permission level (and licenseCode punctuation))
        AddlDevice = 9,              // Activate add'l device on same license, no charge
        // Requests that license host devices
        MakePurchase = 11,           // Complete the purchase
        // Successful Non-Purchase Responses
        ReturnOk = 20,               // Completed non-purchase okay 
        ReturnOkAddlDev = 21,        // Purchase ok, no charge, existing lic, return PinNumber
        ReturnNotActivated = 22,     // Not activated, no paid license found, return Message
        ReturnDeactivate = 23,       // Too many devices, deactivate, return Message
        // Failed Non-Purchase Responses
        ReturnBadArg = 31,           // Invalid city, phone, email, etc, return Message
        ReturnNotFound = 32,         // License not found, return Message
        ReturnNotMatched = 33,       // Name or other info incorrect, return Message
        ReturnLCodeTaken = 34,       // License code already in use by a different user
        ReturnError = 35,            // Internal error, typically all similar license codes in use
        ReturnDenied = 36,           // Caller does not have permission, return Message
        ReturnTimeout = 37,          // Set by Activation when the ws call times out
        // Purchase Responses
        PurchaseOk = 50,             // Purchase went thru, return PinNumber, new LicCode
        PurchaseOkUpgrade = 51,      // Purchase ok, upgrade existing, return PinNumber, new LicCode
        PurchaseFailed = 52,         // Purchase failed, return Message, LicCode
        PurchaseIncomplete = 53,     // Purchase went thru but something else failed, return Message
    }

    /// <summary>
    /// PayPal, etc.
    /// </summary>
    public enum PurchaseAuthority
    {
        Unknown = (int)' ',
        NoCharge = (int)'-',
        PayPalStd = (int)'P',
    }

    /// <summary>
    /// Bitmask (1, 2, 4, 8, 16, 32, 64, 128, 256, etc.) and combo-masks, for product purchases.
    /// </summary>
    public enum ProductBitMask
    {
        None = 0,               // default
        AbleCheckbookStd = 2,
        AbleCheckbookPro = 8,
        AbleCheckbookANY = 31,  // combo mask
        FutureProduct = 32,
    }

    /// <summary>
    /// What is the intent/originator of an interactivity?
    /// </summary>
    public enum InteractivityKind
    {
        Unknown = 0,
        PhoneCall = 1,
        OnlineChat = 2,
        Email = 3,
        InPerson = 4,
        UserAlert = 7,
        RegistrationWs = 8,
        ActivationWs = 9,
        PurchaseHistory = 10,
        PollWs = 11,
        OtherWs = 20,
    }

    /// <summary>
    /// What to reconfigure
    /// </summary>
    public enum ReconfigurationSelection
    {
        Unknown = 0,
        Email = (int)'E',
        Help = (int)'H',
        PayPal = (int)'P',
        WebService = (int)'W',
        Alert = (int)'A',
    }

}
