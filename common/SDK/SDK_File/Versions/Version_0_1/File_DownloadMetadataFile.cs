/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// file download-metadata-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\"
    /// </summary>
    internal class File_DownloadMetadataFile : File_DownloadOperationBase
    {
        public File_DownloadMetadataFile(Arguments _Arguments) : base(_Arguments, "/metadata") { }

        public override string GetCommandName()
        {
            return "download-metadata-file";
        }
    }
}