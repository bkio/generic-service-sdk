


using System.Collections.Generic;
/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License
namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// auth list-users
    /// </summary>
    public class Auth_ListUsers : Command_0_1
    {
        public Auth_ListUsers(Arguments _Arguments) : base(_Arguments, _Arguments.Count == 0)
        {
            if (_Arguments.Count == 0)
            {
                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users").Get();
            }
        }

        public override string GetCommandName()
        {
            return "list-users";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth list-users"),
                (0, "\n")
            };
        }
    }
}