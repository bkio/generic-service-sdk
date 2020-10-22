/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using System.Net;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core auth delete-final-access-right [userId=\"...\"] path=\"...\" apiKey=\"...\"
    /// sdk_core auth delete-final-access-right [userId=\"...\"] path=\"...\" email=\"...\" passwordMd5=\"...\"
    /// sdk_core auth delete-final-access-right [userId=\"...\"] path=\"...\" name=\"...\" passwordMd5=\"...\"
    ///     A single path=\"...\" must be provided.
    /// </summary>
    public class Auth_DeleteFinalAccessRight : Command_0_1
    {
        public Auth_DeleteFinalAccessRight(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path"), new BinaryArgument("apiKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path"), new BinaryArgument("email"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path"), new BinaryArgument("name"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path"), new BinaryArgument("apiKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path"), new BinaryArgument("email"), new BinaryArgument("passwordMd5")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path"), new BinaryArgument("name"), new BinaryArgument("passwordMd5")
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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID + "/access_methods/" + AuthMethod + "/access_rights/" + PathUrlEncoded).Delete();
            }
        }

        public override string GetCommandName()
        {
            return "delete-final-access-right";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth delete-final-access-right [userId=\"...\"] path=\"...\" apiKey=\"...\""),
                (2, "auth delete-final-access-right [userId=\"...\"] path=\"...\" email=\"...\" passwordMd5=\"...\""),
                (2, "auth delete-final-access-right [userId=\"...\"] path=\"...\" name=\"...\" passwordMd5=\"...\""),
                (0, "\tA single path=\"...\" must be provided."),
                (0, "\n")
            };
        }
    }
}