/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d download-metadata-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\"
    /// </summary>
    internal class ThreeD_DownloadMetadataFile : ThreeD_DownloadOperationBase
    {
        public ThreeD_DownloadMetadataFile(Arguments _Arguments) : base(_Arguments, "/metadata") { }

        public override string GetCommandName()
        {
            return "download-metadata-file";
        }
    }
}