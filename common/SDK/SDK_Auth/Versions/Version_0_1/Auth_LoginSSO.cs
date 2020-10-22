/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;


namespace SDK.Versions.V_0_1
{
    public class Auth_LoginSSO : Command_0_1
    {
        public Auth_LoginSSO(Arguments _Arguments) : base(_Arguments, true)
        {
        }

        public override string GetCommandName()
        {
            return "login-sso";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {

            };
        }

        protected override int Perform_Internal_WithoutHttpRequest()
        {
            var Listening = new ManualResetEvent(false);
            var Done = new ManualResetEvent(false);
            var Result = "";

            var LocalhostListenerThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                var LocalhostListener = new HttpListener();
                LocalhostListener.Prefixes.Add("http://localhost:56789/");
                LocalhostListener.Prefixes.Add("http://127.0.0.1:56789/");

                LocalhostListener.Start();

                try
                {
                    Listening.Set();
                }
                catch (Exception) { }

                HttpListenerContext Context = null;
                try
                {
                    Context = LocalhostListener.GetContext();
                    Result = Context.Request.RawUrl.TrimStart('/').TrimStart('?');                        
                }
                catch (Exception e)
                {
                    Utilities.Warning(e.Message + ", trace: " + e.StackTrace);
                }
                finally
                {
                    byte[] Buffer = Encoding.UTF8.GetBytes("<script type=\"text/javascript\">window.open('', '_self', ''); window.close();</script>");

                    try
                    {
                        Context.Response.ContentLength64 = Buffer.Length;
                        if (Buffer.Length > 0)
                        {
                            Context.Response.OutputStream.Write(Buffer, 0, Buffer.Length);
                        }
                    }
                    catch (Exception) { }

                    try
                    {
                        Context.Response.OutputStream.Close();
                    }
                    catch (Exception) { }

                    try
                    {
                        Context.Response.Close();
                    }
                    catch (Exception) { }

                    Thread.Sleep(1000);

                    try
                    {
                        LocalhostListener.Close();
                    }
                    catch (Exception) { }

                    try
                    {
                        Done.Set();
                    }
                    catch (Exception) { }
                }
            });
            LocalhostListenerThread.Start();

            try
            {
                Listening.WaitOne();
                Listening.Close();
            }
            catch (Exception) { }

            if (OpenBrowserWithUrl(BaseApiUrl + "/auth/login/azure", out Process CreatedProcess) == Utilities.FAILURE) return Utilities.FAILURE;

            try
            {
                Done.WaitOne();
                Done.Close();
            }
            catch (Exception) { }

            try
            {
                CreatedProcess?.Close();
                CreatedProcess?.Dispose();
            }
            catch (Exception) {}

            if (Result.StartsWith("error_message="))
            {
                return Utilities.Error(WebUtility.UrlDecode(Result.Substring("error_message=".Length)));
            }
                
            var Splitted = Result.Split('&');
            if (Splitted == null || Splitted.Length < 2)
            {
                return Utilities.Error("Unexpected response from server: " + Result);
            }

            string UserID = null;
            string Token = null;
            foreach (var Current in Splitted)
            {
                if (Current.StartsWith("user_id="))
                {
                    UserID = Current.Substring("user_id=".Length);
                }
                else if (Current.StartsWith("token="))
                {
                    Token = WebUtility.UrlDecode(Current.Substring("token=".Length)).Replace("Bearer+", "Bearer ");
                }
            }

            if (UserID == null || Token == null)
            {
                return Utilities.Error("Unexpected response from server: " + Result);
            }

            try
            {
                File.WriteAllText(UserIDFilePath, UserID);
                File.WriteAllText(TokenFilePath, Token);
            }
            catch (Exception e)
            {
                return Utilities.Error("Login was successful; but file write operation has failed with: " + e.Message + ", please check the SDK has access to write/read your documents folder: " + DocumentsFolderPath);
            }
            return Utilities.Success((new JObject() { ["userId"] = UserID }).ToString());
        }

        private int OpenBrowserWithUrl(string _Url, out Process _CreatedProcess)
        {
            try
            {
                _CreatedProcess = Process.Start(_Url);
            }
            catch
            {
                try
                {
                    switch (Environment.OSVersion.Platform)
                    {
                        case PlatformID.Win32NT:
                        case PlatformID.Win32S:
                        case PlatformID.Win32Windows:
                        case PlatformID.WinCE:
                            _Url = _Url.Replace("&", "^&");
                            _CreatedProcess = Process.Start(new ProcessStartInfo("cmd", $"/c start {_Url}") { CreateNoWindow = true });
                            break;
                        case PlatformID.MacOSX:
                            _CreatedProcess = Process.Start("open", _Url);
                            break;
                        default:
                            _CreatedProcess = Process.Start("xdg-open", _Url);
                            break;
                    }
                }
                catch (Exception e)
                {
                    _CreatedProcess = null;
                    return Utilities.Error("OpenBrowserWithUrl failed. You must have a browser installed in your computer and SDK must have access to create a process.: " + e.Message + ", " + e.StackTrace);
                }
            }
            return Utilities.SUCCESS;
        }
    }
}