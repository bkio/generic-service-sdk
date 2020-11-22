/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d create-model name=\"...\" [metadataPath=\"...\"] [commentsPath=\"...\"]
    ///     metadataPath: File path to the model metadata json structure.
    ///     Example content: [ { \"metadataKey\": \"example\", \"metadata-value\": [\"value1\", \"value2\"] } ]
    ///     commentsPath: File path to the basic model comments text file. Each line in the file will be parsed as a new comment entry.
    /// </summary>
    public class ThreeD_CreateModel : Command_0_1
    {
        public ThreeD_CreateModel(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("name")
                },
                new List<Argument>()
                {
                    new BinaryArgument("name"), new BinaryArgument("metadataPath")
                },
                new List<Argument>()
                {
                    new BinaryArgument("name"), new BinaryArgument("metadataPath"), new BinaryArgument("commentsPath")
                },
                new List<Argument>()
                {
                    new BinaryArgument("name"), new BinaryArgument("commentsPath")
                }
            }))

        {
            if (bParseable)
            {
                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/3d/models");

                string MetadataFilePath = null, MetadataFileContent = null;
                string CommentsFilePath = null, CommentsFileContent = null;

                var ModelName = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                if (AlternativeIx == 1)
                {
                    MetadataFilePath = (_Arguments.First.Value as BinaryArgument).Value;
                }
                else if (AlternativeIx == 2)
                {
                    MetadataFilePath = (_Arguments.First.Value as BinaryArgument).Value;
                    CommentsFilePath = (_Arguments.First.Next.Value as BinaryArgument).Value;
                }
                else if (AlternativeIx == 3)
                {
                    CommentsFilePath = (_Arguments.First.Value as BinaryArgument).Value;
                }

                try
                {
                    if (MetadataFilePath != null) MetadataFileContent = File.ReadAllText(MetadataFilePath);
                    if (CommentsFilePath != null) CommentsFileContent = File.ReadAllText(CommentsFilePath);
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

                var Request = new JObject()
                {
                    ["modelUniqueName"] = ModelName
                };
                if (MetadataFileContent != null)
                {
                    JArray MetadataParsed = null;
                    try
                    {
                        MetadataParsed = JArray.Parse(MetadataFileContent);
                    }
                    catch (Exception)
                    {
                        Utilities.Error("Metadata file content does not represent a valid json structure.");
                        bErrorOccuredInChildConstructor = true;
                        return;
                    }
                    Request["modelMetadata"] = MetadataParsed;
                }
                if (CommentsFileContent != null)
                {
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

                    Request["modelComments"] = Comments;
                }

                CreatedRequest.Put(Request);
            }
        }

        public override string GetCommandName()
        {
            return "create-model";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "3d create-model name=\"...\" [metadataPath=\"...\"] [commentsPath=\"...\"]"),
                (0, "\tmetadataPath: File path to the model metadata json structure."),
                (0, "\tExample content: [ { \"metadataKey\": \"example\", \"metadataValues\": [\"value1\", \"value2\"] } ]"),
                (0, "\tcommentsPath: File path to the basic model comments text file. Each line in the file will be parsed as a new comment entry."),
                (0, "\n")
            };
        }
    }
}