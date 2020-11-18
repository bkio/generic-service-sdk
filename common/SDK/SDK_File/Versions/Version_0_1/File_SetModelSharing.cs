/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// file set-model-sharing modelId=\"...\" [shareWithAll] [userIds=\"...\"] [emails=\"...\"]
    /// </summary>
    public class File_SetModelSharing : Command_0_1
    {
        public File_SetModelSharing(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new UnaryArgument("shareWithAll")
                },
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new UnaryArgument("clearAllSharings")
                },
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("userIds"), new BinaryArgument("emails")
                },
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("emails"), new BinaryArgument("userIds")
                },
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("userIds")
                },
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("emails")
                }
            }))

        {
            if (bParseable)
            {
                var Request = new JObject();

                var ModelID = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                if (AlternativeIx == 0) //shareWithAll
                {
                    Request["shareWithAll"] = true;
                    _Arguments.RemoveFirst();
                }
                else if (AlternativeIx == 1) //clearAllSharings
                {
                    var AsJArray = new JArray();
                    Request["userIds"] = AsJArray;

                    _Arguments.RemoveFirst();
                }
                else
                {
                    for (int i = 0; i < ((AlternativeIx == 2 || AlternativeIx == 3) ? 2 : 1); i++)
                    {
                        var BinArg = _Arguments.First.Value as BinaryArgument;

                        var Splitted = BinArg.Value.Split(',');
                        if (Splitted == null || Splitted.Length == 0)
                        {
                            Utilities.Error(BinArg.Key + " is misconfigured.");
                            return;
                        }
                        var AsJArray = new JArray();
                        foreach (var Cur in Splitted)
                            AsJArray.Add(Cur);

                        Request[BinArg.Key] = AsJArray;

                        _Arguments.RemoveFirst();
                    }
                }

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/file/models/" + ModelID + "/sharing").Post(Request);
            }
        }

        public override string GetCommandName()
        {
            return "set-model-sharing";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "file set-model-sharing modelId=\"...\" [shareWithAll] [userIds=\"...\"] [emails=\"...\"]"),
                (0, "\tshareWithAll: If provided; the model will be shared with all in tenant; also do not provide userIds or emails."),
                (0, "\tYou can provide different(or same) email/userId of users to the userIds and emails field; they are parsed/processed separately."),
                (0, "\tuserIds: Comma-separated user-ids"),
                (0, "\temails: Comma-separated e-mails; append .sso if an e-mail is for a SSO user."),
                (0, "\n")
            };
        }
    }
}