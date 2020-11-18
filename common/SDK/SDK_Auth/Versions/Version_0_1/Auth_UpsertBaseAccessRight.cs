/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// auth upsert-base-access-right [userId=\"...\"] path=\"...\" rights=\"...\"
    ///     A single path=\"...\" rights=\"...\" pair must be provided.
    /// </summary>
    public class Auth_UpsertBaseAccessRight : Command_0_1
    {
        public Auth_UpsertBaseAccessRight(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("path"), new BinaryArgument("rights")
                },
                new List<Argument>()
                {
                    new BinaryArgument("path"), new BinaryArgument("rights")
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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users/" + UserID + "/base_access_rights");

                var Path = (_Arguments.First.Value as BinaryArgument).Value;

                var Rights = (_Arguments.First.Next.Value as BinaryArgument).Value.Split(',');

                var Arr = new JArray();
                if (Rights != null)
                {
                    foreach (var Cur in Rights)
                    {
                        Arr.Add(Cur);
                    }
                }

                CreatedRequest.Put(new JArray()
                {
                    new JObject()
                    {
                        ["wildcardPath"] = Path,
                        ["accessRights"] = Arr
                    }
                });
            }
        }

        public override string GetCommandName()
        {
            return "upsert-base-access-right";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth upsert-base-access-right [userId=\"...\"] path=\"...\" rights=\"...\""),
                (0, "\tA single path=\"...\" rights=\"...\" pair must be provided. Field rights must be separated by comma; for example GET,POST,PUT,DELETE"),
                (0, "\n")
            };
        }
    }
}