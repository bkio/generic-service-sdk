


using System.Collections.Generic;
/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License
namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core auth list-registered-email-addresses
    /// </summary>
    public class Auth_ListEmailAddresses : Command_0_1
    {
        public Auth_ListEmailAddresses(Arguments _Arguments) : base(_Arguments, _Arguments.Count == 0)
        {
            if (_Arguments.Count == 0)
            {
                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/list_registered_email_addresses").Get();
            }
        }

        public override string GetCommandName()
        {
            return "list-registered-email-addresses";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth list-registered-email-addresses"),
                (0, "\n")
            };
        }
    }
}