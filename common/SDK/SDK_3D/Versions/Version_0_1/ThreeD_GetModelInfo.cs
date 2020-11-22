/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d get-model-info modelId=\"...\"
    /// </summary>
    public class ThreeD_GetModelInfo : Command_0_1
    {
        public ThreeD_GetModelInfo(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/3d/models/" + ModelID).Get();
            }
        }

        public override string GetCommandName()
        {
            return "get-model-info";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "3d get-model-info modelId=\"...\""),
                (0, "\n")
            };
        }
    }
}