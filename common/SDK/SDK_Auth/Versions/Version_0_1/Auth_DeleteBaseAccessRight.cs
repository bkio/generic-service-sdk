/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core auth delete-base-access-right [userId=\"...\"] path=\"...\"
    ///     A single path=\"...\" must be provided.
    /// </summary>
    public class Auth_DeleteBaseAccessRight : Command_0_1
    {
        public Auth_DeleteBaseAccessRight(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path")
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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID + "/base_access_rights/" +
                    (_Arguments.First.Value as BinaryArgument).Value).Delete();
            }
        }

        public override string GetCommandName()
        {
            return "delete-base-access-right";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth delete-base-access-right [userId=\"...\"] path=\"...\""),
                (0, "\tA single path=\"...\" must be provided."),
                (0, "\n")
            };
        }
    }
}