/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core auth update-final-access-right [userId=\"...\"] path=\"...\" rights=\"...\" apiKey=\"...\"
    /// sdk_core auth update-final-access-right [userId=\"...\"] path=\"...\" rights=\"...\" email=\"...\" passwordMd5=\"...\"
    /// sdk_core auth update-final-access-right [userId=\"...\"] path=\"...\" rights=\"...\" name=\"...\" passwordMd5=\"...\"
    ///     A single path=\"...\" rights=\"...\" pair must be provided.
    /// </summary>
    public class Auth_UpdateFinalAccessRight : Command_0_1
    {
        public Auth_UpdateFinalAccessRight(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path"), new BinaryArgument("rights"), new BinaryArgument("apiKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path"), new BinaryArgument("rights"), new BinaryArgument("email"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path"), new BinaryArgument("rights"), new BinaryArgument("name"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path"), new BinaryArgument("rights"), new BinaryArgument("apiKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path"), new BinaryArgument("rights"), new BinaryArgument("email"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path"), new BinaryArgument("rights"), new BinaryArgument("name"), new BinaryArgument("passwordMd5")
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

                var PathUrlEncoded = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                var Rights = (_Arguments.First.Value as BinaryArgument).Value.Split(',');
                _Arguments.RemoveFirst();

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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID + "/access_methods/" + AuthMethod + "/access_rights/" + PathUrlEncoded);

                var Arr = new JArray();
                if (Rights != null)
                {
                    foreach (var Cur in Rights)
                    {
                        Arr.Add(Cur);
                    }
                }
                CreatedRequest.Post(Arr);
            }
        }

        public override string GetCommandName()
        {
            return "update-final-access-right";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth update-final-access-right [userId=\"...\"] path=\"...\" rights=\"...\" apiKey=\"...\""),
                (2, "auth update-final-access-right [userId=\"...\"] path=\"...\" rights=\"...\" email=\"...\" passwordMd5=\"...\""),
                (2, "auth update-final-access-right [userId=\"...\"] path=\"...\" rights=\"...\" name=\"...\" passwordMd5=\"...\""),
                (0, "\tA single path=\"...\" rights=\"...\" pair must be provided. Field rights must be separated by comma; for example GET,POST,PUT,DELETE"),
                (0, "\n")
            };
        }
    }
}