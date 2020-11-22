/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d download-geometry-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\"
    /// </summary>
    internal class ThreeD_DownloadGeometryFile : ThreeD_DownloadOperationBase
    {
        public ThreeD_DownloadGeometryFile(Arguments _Arguments) : base(_Arguments, "/geometry") { }

        public override string GetCommandName()
        {
            return "download-geometry-file";
        }
    }
}