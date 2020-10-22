﻿/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core auth get-user-info [userId=\"...\"]
    /// </summary>
    public class Auth_GetUserInfo : Command_0_1
    {
        public Auth_GetUserInfo(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("userId")
                },
                new List<Argument>()
                {
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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID).Get();
            }
        }

        public override string GetCommandName()
        {
            return "get-user-info";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth get-user-info [userId=\"...\"]"),
                (0, "\n")
            };
        }
    }
}