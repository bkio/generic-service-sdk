/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// auth update-user-info [userId=\"...\"] email=\"...\" name=\"...\"
    /// </summary>
    public class Auth_UpdateUserInfo : Command_0_1
    {
        public Auth_UpdateUserInfo(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("email"), new BinaryArgument("name")
                },
                new List<Argument>()
                {
                    new BinaryArgument("email"), new BinaryArgument("name")
                }
            }))

        {
            if (bParseable)
            {
                string UserID;
                if (AlternativeIx == 1)
                {
                    if (UserIDCached == null)
                    {
                        Utilities.Error("You must login to have the userId to be cached and used in the requests.");
                        bErrorOccuredInChildConstructor = true;
                        return;
                    }
                    UserID = UserIDCached;
                }
                else
                {
                    UserID = (_Arguments.First.Value as BinaryArgument).Value;
                    _Arguments.RemoveFirst();
                }

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID).Post(
                    new JObject()
                    {
                        ["userEmail"] = (_Arguments.First.Value as BinaryArgument).Value,
                        ["userName"] = (_Arguments.First.Next.Value as BinaryArgument).Value
                    });
            }
        }

        public override string GetCommandName()
        {
            return "update-user-info";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth update-user-info [userId=\"...\"] email=\"...\" name=\"...\""),
                (0, "\n")
            };
        }
    }
}