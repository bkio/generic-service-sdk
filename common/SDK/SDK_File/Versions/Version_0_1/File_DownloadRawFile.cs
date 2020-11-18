/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// file download-raw-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\"
    /// </summary>
    internal class File_DownloadRawFile : File_DownloadOperationBase
    {
        public File_DownloadRawFile(Arguments _Arguments) : base(_Arguments, "/raw") { }

        public override string GetCommandName()
        {
            return "download-raw-file";
        }
    }
}