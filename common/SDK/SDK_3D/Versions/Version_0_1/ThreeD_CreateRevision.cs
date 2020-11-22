/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d create-revision modelId=\"...\" name=\"...\" [commentsPath=\"...\"]
    /// </summary>
    public class ThreeD_CreateRevision : Command_0_1
    {
        public ThreeD_CreateRevision(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("name"),
                },
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("name"), new BinaryArgument("commentsPath")
                }
            }))

        {
            if (bParseable)
            {
                var Request = new JObject();

                var ModelID = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                Request["revisionName"] = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                if (AlternativeIx == 1)
                {
                    string CommentsFileContent = null;

                    var CommentsFilePath = (_Arguments.First.Value as BinaryArgument).Value;
                    try
                    {
                        CommentsFileContent = File.ReadAllText(CommentsFilePath);
                    }
                    catch (Exception e)
                    {
                        if (e is FileNotFoundException)
                        {
                            Utilities.Error("File not found.");
                        }
                        else
                        {
                            Utilities.Error("File read operation has failed with: " + e.Message + ", please check the SDK has access to write/read the file path.");
                        }

                        bErrorOccuredInChildConstructor = true;
                        return;
                    }

                    var Comments = new JArray();

                    var Splitted = CommentsFileContent.Split('\n');
                    if (Splitted == null || Splitted.Length == 0)
                    {
                        Comments.Add(CommentsFileContent);
                    }
                    else
                    {
                        foreach (var Cur in Splitted)
                        {
                            Comments.Add(Cur);
                        }
                    }

                    Request["revisionComments"] = Comments;
                }

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/3d/models/" + ModelID + "/revisions").Put(Request);
            }
        }

        public override string GetCommandName()
        {
            return "create-revision";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "3d create-revision modelId=\"...\" name=\"...\" [commentsPath=\"...\"]"),
                (0, "\n")
            };
        }
    }
}