/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    public class Auth_LoginOther : Command_0_1
    {
        public Auth_LoginOther(Arguments _Arguments, Auth_Login.ELoginType _LoginType) 
            : base(_Arguments, true)
        {
            CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/login");

            var Request = new JObject();
            if (_LoginType == Auth_Login.ELoginType.ApiKey)
            {
                Request["apiKey"] = (_Arguments.First.Value as BinaryArgument).Value;
            }
            else if (_LoginType == Auth_Login.ELoginType.EmailPasswordMD5)
            {
                Request["userEmail"] = (_Arguments.First.Value as BinaryArgument).Value;
                Request["passwordMd5"] = (_Arguments.First.Next.Value as BinaryArgument).Value;
            }
            else if (_LoginType == Auth_Login.ELoginType.UsernamePasswordMD5)
            {
                Request["userName"] = (_Arguments.First.Value as BinaryArgument).Value;
                Request["passwordMd5"] = (_Arguments.First.Next.Value as BinaryArgument).Value;
            }
            CreatedRequest.Post(Request);
        }

        public Auth_LoginOther(Arguments _Arguments)
            : base(_Arguments, true)
        {
            CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/login");
        }

        public override string GetCommandName()
        {
            return "login-other";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {

            };
        }

        protected override int Perform_Internal_WithHttpRequest(int _ResultHttpCode, JObject _ResultJson) 
        {
            if (_ResultHttpCode < 400)
            {
                var UserID = (string)_ResultJson["userId"];
                var Token = (string)_ResultJson["token"];
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

            return Utilities.Error(_ResultJson.ToString(Formatting.Indented));
        }
    }
}