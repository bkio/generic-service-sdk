/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d download-hierarchy-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\"
    /// </summary>
    internal class ThreeD_DownloadHierarchyFile : ThreeD_DownloadOperationBase
    {
        public ThreeD_DownloadHierarchyFile(Arguments _Arguments) : base(_Arguments, "/hierarchy") { }

        public override string GetCommandName()
        {
            return "download-hierarchy-file";
        }
    }
}