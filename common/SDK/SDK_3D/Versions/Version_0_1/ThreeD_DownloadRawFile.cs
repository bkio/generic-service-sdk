/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d download-raw-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\"
    /// </summary>
    internal class ThreeD_DownloadRawFile : ThreeD_DownloadOperationBase
    {
        public ThreeD_DownloadRawFile(Arguments _Arguments) : base(_Arguments, "/raw") { }

        public override string GetCommandName()
        {
            return "download-raw-file";
        }
    }
}