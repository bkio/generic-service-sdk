/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SDK.Versions;

namespace SDK
{
    public class Program
    {
        public static int Main(string[] _Args)
        {
            Version_0_1.LoadCommandAssemblies(new string[] 
            {
                "SDK_Auth.dll",
                "SDK_3D.dll"
            });
            var Result = Main_Internal(_Args);
            if (Result == Utilities.SUCCESS_NO_JSON_OUTPUT_EXCEPTION)
            {
                return Utilities.SUCCESS;
            }

            var MessagesJArray = new JArray();
            var Json = new JObject()
            {
                ["sdkExecResult"] = Result == Utilities.SUCCESS ? "success" : "failure",
                ["sdkExecMessages"] = MessagesJArray
            };
            foreach (var Message in Utilities.Messages)
            {
                try
                {
                    var ParsedMessage = JObject.Parse(Message);
                    MessagesJArray.Add(ParsedMessage);
                }
                catch (Exception)
                {
                    MessagesJArray.Add(Message);
                }
            }
            Utilities.Print(Json.ToString(Newtonsoft.Json.Formatting.Indented));

            return Result;
        }
        private static int Main_Internal(string[] _Args)
        {
            if (Arguments.ParseArguments(_Args, out Arguments _Arguments) == Utilities.FAILURE) return Utilities.FAILURE;
            if (GetDeploymentName(_Arguments, out string DeploymentName) == Utilities.FAILURE) return Utilities.FAILURE;

            if (Version.InitializeVersion(_Arguments, DeploymentName, out Version _Version, out string _BaseApiUrl) == Utilities.FAILURE) return Utilities.FAILURE;
            return _Version.Operate(_Arguments, _BaseApiUrl);
        }
        
        private static int GetDeploymentName(Arguments _Arguments, out string _DeploymentName)
        {
            _DeploymentName = null;

            var DNs = _Arguments.Contains(Argument.Type.Binary, Arguments.EContainsFunctionInput.UnaryValueOrBinaryKey, "deploymentName");

            if (DNs.Count > 1) return Utilities.Error("Multiple deployment arguments cannot be provided.");
            if (DNs.Count == 0) _DeploymentName = null;
            else
            {
                _DeploymentName = (DNs[0].Value as BinaryArgument).Value;
            }

            return Utilities.SUCCESS;
        }
    }
}