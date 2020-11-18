/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// auth create-access-method [userId=\"...\"] newApiKey
    /// auth create-access-method [userId=\"...\"] email=\"...\" passwordMd5=\"...\"
    /// auth create-access-method [userId=\"...\"] name=\"...\" passwordMd5=\"...\"
    /// </summary>
    public class Auth_CreateAccessMethod : Command_0_1
    {
        public Auth_CreateAccessMethod(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new UnaryArgument("newApiKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("email"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("name"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new UnaryArgument("newApiKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("email"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("name"), new BinaryArgument("passwordMd5")
                }
            }, true))

        {
            if (bParseable)
            {
                string UserID;
                if (AlternativeIx >= 3)
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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID + "/access_methods");

                var Request = new JObject();
                if (AlternativeIx == 0 || AlternativeIx == 3)
                {
                    Request["method"] = "apiKeyMethod";
                }
                else if (AlternativeIx == 1 || AlternativeIx == 4)
                {
                    Request["method"] = "userEmailPasswordMethod";
                    Request["userEmail"] = (_Arguments.First.Value as BinaryArgument).Value;
                    Request["passwordMd5"] = (_Arguments.First.Next.Value as BinaryArgument).Value;
                }
                else
                {
                    Request["method"] = "userNamePasswordMethod";
                    Request["userName"] = (_Arguments.First.Value as BinaryArgument).Value;
                    Request["passwordMd5"] = (_Arguments.First.Next.Value as BinaryArgument).Value;
                }
                CreatedRequest.Put(Request);
            }
        }

        public override string GetCommandName()
        {
            return "create-access-method";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth create-access-method [userId=\"...\"] newApiKey"),
                (2, "auth create-access-method [userId=\"...\"] email=\"...\" passwordMd5=\"...\""),
                (2, "auth create-access-method [userId=\"...\"] name=\"...\" passwordMd5=\"...\""),
                (0, "\n")
            };
        }
    }
}