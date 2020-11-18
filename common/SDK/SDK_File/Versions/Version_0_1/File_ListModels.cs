/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// file list-models
    /// </summary>
    public class File_ListModels : Command_0_1
    {
        public File_ListModels(Arguments _Arguments) : base(_Arguments, _Arguments.Count == 0)
        {
            if (_Arguments.Count == 0)
            {
                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/file/models").Get();
            }
        }

        public override string GetCommandName()
        {
            return "list-models";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "file list-models"),
                (0, "\n")
            };
        }
    }
}