﻿/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core file delete-files modelId=\"...\" revisionIndex=\"...\" versionIndex=\"...\"
    /// </summary>
    public class File_DeleteFiles : Command_0_1
    {
        public File_DeleteFiles(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("revisionIndex"), new BinaryArgument("versionIndex")
                }
            }))

        {
            if (bParseable)
            {
                var ModelID = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                if (!int.TryParse((_Arguments.First.Value as BinaryArgument).Value, out int RevisionIndex) || RevisionIndex < 0)
                {
                    bErrorOccuredInChildConstructor = true;
                    Utilities.Error("Revision index must be a natural number.");
                    return;
                }
                _Arguments.RemoveFirst();

                if (!int.TryParse((_Arguments.First.Value as BinaryArgument).Value, out int VersionIndex) || VersionIndex < 0)
                {
                    bErrorOccuredInChildConstructor = true;
                    Utilities.Error("Version index must be a natural number.");
                    return;
                }
                _Arguments.RemoveFirst();

                //Deletes all files; raw, processed.
                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/file/models/" + ModelID + "/revisions/" + RevisionIndex + "/versions/" + VersionIndex + "/raw").Delete();
            }
        }

        public override string GetCommandName()
        {
            return "delete-files";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "file delete-files modelId=\"...\" revisionIndex=\"...\" versionIndex=\"...\""),
                (0, "\n")
            };
        }
    }
}