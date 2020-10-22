/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core file download-geometry-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\" versionIndex=\"...\"
    /// </summary>
    internal class File_DownloadGeometryFile : File_DownloadOperationBase
    {
        public File_DownloadGeometryFile(Arguments _Arguments) : base(_Arguments, "/geometry") { }

        public override string GetCommandName()
        {
            return "download-geometry-file";
        }
    }
}