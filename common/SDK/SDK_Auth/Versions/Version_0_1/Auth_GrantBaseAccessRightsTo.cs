/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;


namespace SDK.Versions
{
    /// <summary>
    /// sdk_core auth grant-base-access-rights-to [userId=\"...\"] apiKey=\"...\"
    /// sdk_core auth grant-base-access-rights-to [userId=\"...\"] email=\"...\" passwordMd5=\"...\"
    /// sdk_core auth grant-base-access-rights-to [userId=\"...\"] name=\"...\" passwordMd5=\"...\"
    /// </summary>
    public class Auth_GrantBaseAccessRightsTo : Command_0_1
    {
        public Auth_GrantBaseAccessRightsTo(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
            new List<Argument>()
            {
                new BinaryArgument("userId"), new BinaryArgument("apiKey")
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
                new BinaryArgument("apiKey")
            },
            new List<Argument>()
            {
                new BinaryArgument("email"), new BinaryArgument("passwordMd5")
            },
            new List<Argument>()
            {
                new BinaryArgument("name"), new BinaryArgument("passwordMd5")
            }
            }))

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

                string AuthMethod;

                if (AlternativeIx == 0 || AlternativeIx == 3)
                {
                    AuthMethod = (_Arguments.First.Value as BinaryArgument).Value;
                }
                else if (AlternativeIx == 1 || AlternativeIx == 4)
                {
                    AuthMethod =
                        (_Arguments.First.Value as BinaryArgument).Value +
                        (_Arguments.First.Next.Value as BinaryArgument).Value;
                }
                else
                {
                    AuthMethod =
                        (_Arguments.First.Value as BinaryArgument).Value +
                        (_Arguments.First.Next.Value as BinaryArgument).Value;
                }
                AuthMethod = WebUtility.UrlEncode(AuthMethod);

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID + "/access_methods/" + AuthMethod + "/access_rights/grant_base_access_rights")
                    .Post(new JObject());
            }
        }

        public override string GetCommandName()
        {
            return "grant-base-access-rights-to";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth grant-base-access-rights-to [userId=\"...\"] apiKey=\"...\""),
                (2, "auth grant-base-access-rights-to [userId=\"...\"] email=\"...\" passwordMd5=\"...\""),
                (2, "auth grant-base-access-rights-to [userId=\"...\"] name=\"...\" passwordMd5=\"...\""),
                (0, "\n")
            };
        }
    }
}