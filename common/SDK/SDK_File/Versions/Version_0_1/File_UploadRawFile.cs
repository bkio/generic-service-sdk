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
    /// <summary>
    /// file upload-raw-file sourcePath=\"...\" modelId=\"...\" revisionIndex=\"...\" versionIndex=\"...\" fileEntryName=\"...\" fileType=\"...\" dataSource=\"...\" [zipMainAssemblyFileNameIfAny=\"...\"]
    /// </summary>
    public class File_UploadRawFile : Command_0_1
    {
        public File_UploadRawFile(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("sourcePath"), new BinaryArgument("modelId"), new BinaryArgument("revisionIndex"), new BinaryArgument("versionIndex"), new BinaryArgument("fileEntryName"), new BinaryArgument("fileType"), new BinaryArgument("dataSource")
                },
                new List<Argument>()
                {
                    new BinaryArgument("sourcePath"), new BinaryArgument("modelId"), new BinaryArgument("revisionIndex"), new BinaryArgument("versionIndex"), new BinaryArgument("fileEntryName"), new BinaryArgument("fileType"), new BinaryArgument("dataSource"), new BinaryArgument("zipMainAssemblyFileNameIfAny")
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

                if (!int.TryParse((_Arguments.First.Value as BinaryArgument).Value, out int VersionIndex) || VersionIndex < 0)
                {
                    bErrorOccuredInChildConstructor = true;
                    Utilities.Error("Version index must be a natural number.");
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

                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/file/models/" + ModelID + "/revisions/" + RevisionIndex + "/versions/" + VersionIndex + "/raw").Post(Request);
            }
        }

        protected string SourcePath;

        protected override int Perform_Internal_WithHttpRequest(int _ResultHttpCode, JObject _ResultJson)
        {
            if (_ResultHttpCode >= 400) return Utilities.Error(_ResultJson.ToString(Formatting.Indented));

            var UploadSignedUrl = (string)_ResultJson["fileUploadUrl"];
            var UploadContentType = (string)_ResultJson["fileUploadContentType"];

            string InnerFailureMessage = "";
            bool bUploadSucceed = true;
            int EntireUploadFailureCount = 0;
            do
            {
                try
                {
                    using (var FileReadStream = new FileStream(SourcePath, FileMode.Open, FileAccess.Read))
                    {
                        var Request = (HttpWebRequest)WebRequest.Create(UploadSignedUrl);
                        Request.AllowReadStreamBuffering = false;
                        Request.AllowWriteStreamBuffering = false;
                        Request.Method = "PUT";
                        Request.ContentType = UploadContentType;
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
                                        InnerFailureMessage = e.Message;
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
                        return Utilities.Error("File not found.");
                    }
                    return Utilities.Error("Operation has failed with: " + e.Message);
                }

            } while (!bUploadSucceed && ++EntireUploadFailureCount < 5 && FailedChunkWait());

            if (!bUploadSucceed)
            {
                return Utilities.Error("Operation has exhausted retrying failed upload attempt: " + InnerFailureMessage);
            }
            return Utilities.Success(new JObject() { ["result"] = "success" }.ToString());
        }
        private bool FailedChunkWait() { Thread.Sleep(1000); return true; }

        public override string GetCommandName()
        {
            return "upload-raw-file";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "file upload-raw-file sourcePath=\"...\" modelId=\"...\" revisionIndex=\"...\" versionIndex=\"...\" fileEntryName=\"...\" fileType=\"...\" dataSource=\"...\" [zipMainAssemblyFileNameIfAny=\"...\"]"),
                (0, "\tdataSource is name of the software the CAD file has been designed in. Example: Aveva, Solidworks etc."),
                (0, "\n")
            };
        }
    }
}