/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using Google.Api.Gax;
using Google.Apis.Http;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d upload-raw-file sourcePath=\"...\" modelId=\"...\" revisionIndex=\"...\" fileEntryName=\"...\" fileType=\"...\" dataSource=\"...\" [zipMainAssemblyFileNameIfAny=\"...\"]
    /// </summary>
    public class ThreeD_UploadRawFile : Command_0_1
    {
        public ThreeD_UploadRawFile(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("sourcePath"), new BinaryArgument("modelId"), new BinaryArgument("revisionIndex"), new BinaryArgument("fileEntryName"), new BinaryArgument("fileType"), new BinaryArgument("dataSource")
                },
                new List<Argument>()
                {
                    new BinaryArgument("sourcePath"), new BinaryArgument("modelId"), new BinaryArgument("revisionIndex"), new BinaryArgument("fileEntryName"), new BinaryArgument("fileType"), new BinaryArgument("dataSource"), new BinaryArgument("zipMainAssemblyFileNameIfAny")
                }
            }))

        {
            if (bParseable)
            {
                SourcePath = (_Arguments.First.Value as BinaryArgument).Value;
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

                var Request = new JObject
                {
                    ["generateUploadUrl"] = true,
                    ["fileEntryName"] = (_Arguments.First.Value as BinaryArgument).Value,
                    ["fileEntryFileType"] = (_Arguments.First.Next.Value as BinaryArgument).Value,
                    ["dataSource"] = (_Arguments.First.Next.Next.Value as BinaryArgument).Value
                };

                if (AlternativeIx == 1)
                {
                    Request["zipMainAssemblyFileNameIfAny"] = (_Arguments.First.Next.Next.Next.Value as BinaryArgument).Value;
                }

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/3d/models/" + ModelID + "/revisions/" + RevisionIndex + "/raw").Post(Request);
            }
        }

        protected string SourcePath;

        protected override int Perform_Internal_WithHttpRequest(int _ResultHttpCode, JObject _ResultJson)
        {
            if (_ResultHttpCode >= 400) return Utilities.Error(_ResultJson.ToString(Formatting.Indented));

            var UploadSignedUrl = (string)_ResultJson["fileUploadUrl"];
            var UploadContentType = (string)_ResultJson["fileUploadContentType"];

            //Google multi-part upload
            if (UploadSignedUrl.StartsWith("https://storage.googleapis.com/"))
            {
                if (!GoogleSignedUrlUpload(UploadSignedUrl, UploadContentType, out int FailureCode))
                {
                    return FailureCode;
                }
            }
            else if (!OtherUrlUpload(UploadSignedUrl, UploadContentType, out int FailureCode))
            {
                return FailureCode;
            }

            return Utilities.Success(new JObject() { ["result"] = "success" }.ToString());
        }

        private bool GoogleSignedUrlUpload(string _UploadSignedUrl, string _UploadContentType, out int _FailureCode)
        {
            _FailureCode = 500;

            HttpClient Client = null;

            try
            {
                Client = new HttpClientFactory().CreateHttpClient(new CreateHttpClientArgs { ApplicationName = "ResumableUpload", GZipEnabled = true, GoogleApiClientHeader = DefaultGoogleApiClientHeader });
                Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", _UploadContentType);

                using (var FileReadStream = new FileStream(SourcePath, FileMode.Open, FileAccess.Read))
                {
                    var UploadRequest = SignedUrlResumableUpload.Create(_UploadSignedUrl, FileReadStream, new ResumableUploadOptions()
                    {
                        HttpClient = Client
                    });
                    var Result = UploadRequest.Upload();

                    if (Result.Status == UploadStatus.Completed)
                        return true;
                    
                    if (Result.Exception != null)
                    {
                        _FailureCode = Utilities.Error("Operation has failed. Status: " + Result.Status.ToString() + ", bytes sent: " + Result.BytesSent + " with message: " + Result.Exception.Message);
                    }
                    else
                    {
                        _FailureCode = Utilities.Error("Operation has failed. Status: " + Result.Status.ToString() + ", bytes sent: " + Result.BytesSent);
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                _FailureCode = Utilities.Error("Operation has failed with: " + e.Message);
                return false;
            }
            finally
            {
                try { Client?.Dispose(); } catch (Exception) { }
            }
        }
        private static readonly string DefaultGoogleApiClientHeader = new VersionHeaderBuilder().AppendDotNetEnvironment().AppendAssemblyVersion("gdcl", typeof(ResumableUpload)).ToString();

        private bool FailedChunkWait() { Thread.Sleep(1000); return true; }

        private bool OtherUrlUpload(string _UploadSignedUrl, string _UploadContentType, out int _FailureCode)
        {
            _FailureCode = 500;

            string ExhaustedLastErrorMessage = "";

            bool bUploadSucceed = true;
            int EntireUploadFailureCount = 0;
            do
            {
                try
                {
                    using (var FileReadStream = new FileStream(SourcePath, FileMode.Open, FileAccess.Read))
                    {
                        var Request = (HttpWebRequest)WebRequest.Create(_UploadSignedUrl);
                        Request.AllowReadStreamBuffering = false;
                        Request.AllowWriteStreamBuffering = false;
                        Request.Method = "PUT";
                        Request.ContentType = _UploadContentType;
                        Request.ContentLength = new FileInfo(SourcePath).Length;
                        Request.ServerCertificateValidationCallback = (a, b, c, d) => true;

                        using (var RequestStream = Request.GetRequestStream())
                        {
                            int BytesRead = 0;
                            byte[] Buffer = new byte[10 * 1024 * 1024];

                            while ((BytesRead = FileReadStream.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                bool bSuccess = true;
                                int FailureCount = 0;
                                do
                                {
                                    try
                                    {
                                        RequestStream.Write(Buffer, 0, BytesRead);
                                    }
                                    catch (Exception e)
                                    {
                                        bSuccess = false;
                                        ExhaustedLastErrorMessage = e.Message;
                                    }

                                } while (!bSuccess && ++FailureCount < 60 && FailedChunkWait());

                                if (!bSuccess)
                                {
                                    bUploadSucceed = false;
                                    break;
                                }
                            }
                        }
                        using (var Response = (HttpWebResponse)Request.GetResponse())
                        {
                            if ((int)Response.StatusCode >= 400)
                            {
                                try
                                {
                                    using (var ResponseStream = Response.GetResponseStream())
                                    {
                                        using (var Reader = new StreamReader(ResponseStream))
                                        {
                                            throw new Exception(Reader.ReadToEnd());
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    throw new Exception(((int)Response.StatusCode).ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e is FileNotFoundException)
                    {
                        _FailureCode = Utilities.Error("File not found.");
                        return false;
                    }
                    _FailureCode = Utilities.Error("Operation has failed with: " + e.Message);
                    return false;
                }

            } while (!bUploadSucceed && ++EntireUploadFailureCount < 5 && FailedChunkWait());

            if (!bUploadSucceed)
            {
                _FailureCode = Utilities.Error("Operation has exhausted retrying failed upload attempt: " + (ExhaustedLastErrorMessage.Length > 0 ? ExhaustedLastErrorMessage : "-Empty-"));
                return false;
            }
            return true;
        }

        public override string GetCommandName()
        {
            return "upload-raw-file";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "3d upload-raw-file sourcePath=\"...\" modelId=\"...\" revisionIndex=\"...\" fileEntryName=\"...\" fileType=\"...\" dataSource=\"...\" [zipMainAssemblyFileNameIfAny=\"...\"]"),
                (0, "\tdataSource is name of the software the CAD file has been designed in. Example: Aveva, Solidworks etc."),
                (0, "\n")
            };
        }
    }
}