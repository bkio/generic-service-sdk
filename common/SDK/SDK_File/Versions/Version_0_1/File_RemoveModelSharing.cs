/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core file remove-model-sharing modelId=\"...\" [userId=\"...\"]
    /// </summary>
    public class File_RemoveModelSharing : Command_0_1
    {
        public File_RemoveModelSharing(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("userId")
                },
                new List<Argument>()
                {
                    new BinaryArgument("modelId")
                }
            }))

        {
            if (bParseable)
            {
                var ModelID = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/file/models/" + ModelID + "/remove_sharing_from/user_id/" + UserID).Delete();
            }
        }

        public override string GetCommandName()
        {
            return "remove-model-sharing";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "file remove-model-sharing modelId=\"...\" [userId=\"...\"]"),
                (0, "\n")
            };
        }
    }
}
