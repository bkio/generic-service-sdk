/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SDK.Versions.V_0_1
{
    public class File_DownloadOperationBase : Command_0_1
    {
        protected File_DownloadOperationBase(Arguments _Arguments, string _UrlAfterVersionIxPath)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("destinationPath"), new BinaryArgument("modelId"), new BinaryArgument("revisionIndex")
                }
            }))

        {
            if (bParseable)
            {
                DestinationPath = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                var ModelID = (_Arguments.First.Value as BinaryArgument).Value;
                _Arguments.RemoveFirst();

                if (!int.TryParse((_Arguments.First.Value as BinaryArgument).Value, out int RevisionIndex) || RevisionIndex < 0)
                {
                    bErrorOccuredInChildConstructor = true;
                    Utilities.Error("Revision index must be a natural number.");
                    return;
                }
                _Arguments.RemoveFirst();

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/file/models/" + ModelID + "/revisions/" + RevisionIndex + _UrlAfterVersionIxPath).Get();
            }
        }

        public File_DownloadOperationBase(Arguments _Arguments)
           : base(_Arguments, false)
        {

        }

        protected string DestinationPath;

        protected override int Perform_Internal_WithHttpRequest(int _ResultHttpCode, JObject _ResultJson)
        {
            if (_ResultHttpCode >= 400) return Utilities.Error(_ResultJson.ToString(Formatting.Indented));

            var DownloadSignedUrl = (string)_ResultJson["fileDownloadUrl"];

            string InnerFailureMessage = "";
            bool bDownloadSucceed;
            int EntireDownloadFailureCount = 0;
            do
            {
                bDownloadSucceed = true;
                try
                {
                    using (var FileWriteStream = new FileStream(DestinationPath, FileMode.Create, FileAccess.Write))
                    {
                        var DownloadRequest = ((HttpWebRequest)WebRequest.Create(DownloadSignedUrl));
                        DownloadRequest.AllowReadStreamBuffering = false;
                        DownloadRequest.AllowWriteStreamBuffering = false;
                        DownloadRequest.ServerCertificateValidationCallback = (a, b, c, d) => true;

                        using (var Response = DownloadRequest.GetResponse())
                        {
                            using (var ResponseStream = Response.GetResponseStream())
                            {
                                byte[] Buffer = new byte[10 * 1024 * 1024];
                                do
                                {
                                    int BytesRead = 0;

                                    bool bSuccess = true;
                                    int FailureCount = 0;
                                    do
                                    {
                                        try
                                        {
                                            BytesRead = ResponseStream.Read(Buffer, 0, Buffer.Length);
                                        }
                                        catch (Exception e)
                                        {
                                            bSuccess = false;
                                            InnerFailureMessage = e.Message;
                                        }

                                    } while (!bSuccess && ++FailureCount < 60 && FailedChunkWait());

                                    if (!bSuccess)
                                    {
                                        bDownloadSucceed = false;
                                        break;
                                    }
                                    if (BytesRead <= 0) break;

                                    FileWriteStream.Write(Buffer, 0, BytesRead);

                                } while (true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    return Utilities.Error("Operation has failed with: " + e.Message);
                }

            } while (!bDownloadSucceed && ++EntireDownloadFailureCount < 5 && FailedChunkWait());

            if (!bDownloadSucceed)
            {
                try
                {
                    File.Delete(DestinationPath);
                }
                catch (Exception) { }
                return Utilities.Error("Operation has exhausted retrying failed download attempt: " + InnerFailureMessage);
            }
            return Utilities.Success(new JObject() { ["result"] = "success" }.ToString());
        }
        private bool FailedChunkWait() { Thread.Sleep(1000); return true; }

        public override string GetCommandName()
        {
            return "download-file-operations";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "file download-raw-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\""),
                (2, "file download-hierarchy-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\""),
                (2, "file download-geometry-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\""),
                (2, "file download-metadata-file destinationPath=\"...\" modelId=\"...\" revisionIndex=\"...\""),
                (0, "\n")
            };
        }
    }
}