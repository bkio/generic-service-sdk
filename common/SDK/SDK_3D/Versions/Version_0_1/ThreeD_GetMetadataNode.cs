/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d get-metadata-node modelId=\"...\" revisionIndex=\"...\" nodeId=\"...\"
    /// </summary>
    public class ThreeD_GetMetadataNode : Command_0_1
    {
        public ThreeD_GetMetadataNode(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("modelId"), new BinaryArgument("revisionIndex"), new BinaryArgument("nodeId")
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

                if (!ulong.TryParse((_Arguments.First.Value as BinaryArgument).Value, out ulong NodeID) || NodeID < 0)
                {
                    bErrorOccuredInChildConstructor = true;
                    Utilities.Error("Node ID must be a natural number.");
                    return;
                }
                _Arguments.RemoveFirst();

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/3d/models/" + ModelID + "/revisions/" + RevisionIndex + "/metadata/nodes/" + NodeID).Get();
            }
        }

        public override string GetCommandName()
        {
            return "get-metadata-node";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "3d get-metadata-node modelId=\"...\" revisionIndex=\"...\" nodeId=\"...\""),
                (0, "\n")
            };
        }
    }
}