/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SDK.Versions
{
    public partial class Version_0_1 : Version
    {
        public static Dictionary<string, Type> LoadedCommands = new Dictionary<string, Type>();
        public Version_0_1() : base(EVersion.V_0_1)
        {
        }

        protected override int DeploymentToApiBaseUrl(string _Deployment, out string _BaseApiUrl)
        {
            if (_Deployment == null)
            {
                if (Deployment_SetExclusivelyInPreviousSession != null)
                {
                    var Result = DeploymentToApiBaseUrl(Deployment_SetExclusivelyInPreviousSession, out _BaseApiUrl);
                    if (Result == Utilities.FAILURE)
                    {
                        try
                        {
                            File.Delete(DeploymentCached_FilePath);
                        }
                        catch (Exception) { }
                        return Utilities.Error("Invalid cached deployment argument. Please try again.");
                    }
                    return Result;
                }
                _Deployment = Defines.DEPLOYMENT_NAME;
            }

            if (_Deployment == "prod") _Deployment = "master";
            else if (_Deployment == "dev") _Deployment = "development";

            if (_Deployment == "master" || _Deployment == "development")
            {
                _BaseApiUrl = "https://api-" + _Deployment + ".[your-domain]";
            }
            else if (!_Deployment.StartsWith("https://") || !_Deployment.EndsWith(".run.app"))
            {
                _BaseApiUrl = null;
                return Utilities.Error("Argument deployment can be master|prod or development|dev or https://[your-public-cloud-service-url].run.app. BaseApiUrl: "+ _Deployment);
            }
            else
            {
                _BaseApiUrl = _Deployment;
            }

            return Utilities.SUCCESS;
        }

        public static void LoadCommandAssemblies(string[] AssemblyNames)
        {
            Arguments.ParseArguments(new string[] { "" }, out Arguments _BlankArg);
            for (int i = 0; i < AssemblyNames.Length; ++i)
            {
                Assembly Asm = Assembly.Load(File.ReadAllBytes(AssemblyNames[i]));
                Type[] AllTypes = Asm.GetTypes();

                for(int t = 0; t < AllTypes.Length; ++t)
                {
                    if(AllTypes[t].BaseType.FullName == typeof(Command_0_1).FullName)
                    {
                        Type Current = AllTypes[t];

                        Command_0_1 Instance = (Command_0_1)Activator.CreateInstance(Current, new object[] { _BlankArg });
                        LoadedCommands.Add(Instance.GetCommandName(), Current);
                    }
                }
            }
        }

        protected override List<ValueTuple<int, string>> GetHelpLines()
        {
            List<ValueTuple<int, string>> Result = new List<ValueTuple<int, string>>()
            {
                (0, "Note: Optional arguments are shown as [argument(s)]"),
                (0, "Note: [argument(s)]... means; at least one -argument(s)- must be provided; but also you can provide multiple."),
                (0, "Note: (argument1 or argument2 ...) means; only one of -argument(no)-s must be provided."),
                (0, "Note: If userId argument is not provided; command will be run for the user logged in."),
                (0, "\n"),
                (0, "Settings - commands"),
                (1, /*_______________________*/""),
                (0, "\n"),
                (2, "set versionName=\"...\""),
                (0, "\tAvailable versions: " + string.Join(", ", StringToEnum.Keys)),
                (2, "set deploymentName=\"...\""),
                (0, "\n")
            };
            
            foreach (Type Command in LoadedCommands.Values)
            {
                Command_0_1 Cmd = (Command_0_1)Activator.CreateInstance(Command, "");
                Result.AddRange(Cmd.GetHelpLines());
            }

            return Result;
        }

        protected override int OperateVersionSpecific(Arguments _Arguments)
        {
            //Set static variables
            Command_0_1.BaseApiUrl = BaseApiUrl;
            Command_0_1.UserIDCached = UserIDCached;
            Command_0_1.DocumentsFolderPath = DocumentsFolderPath;
            Command_0_1.UserIDFilePath = UserIDFilePath;
            Command_0_1.TokenFilePath = TokenFilePath;
            ApiHttpRequest.TokenCached = TokenCached;
            ApiHttpRequest.OnTokenIsChangedByServer = (string _NewToken) =>
            {
                TokenCached = _NewToken;
                try
                {
                    File.WriteAllText(TokenFilePath, TokenCached);
                }
                catch (Exception e)
                {
                    Utilities.Warning(e.Message + ", trace: " + e.StackTrace);
                }
            };

            //set or auth or file
            if (_Arguments.First.Value.ArgumentType != Argument.Type.Unary) 
                return Utilities.Error("Invalid argument. Expected: set, auth, file, custom-procedures; given argument is: " + _Arguments.First.Value.ToString());

            var Current = (_Arguments.First.Value as UnaryArgument).Value;

            _Arguments.RemoveFirst();
            if (_Arguments.Count == 0) return Utilities.Error("More arguments must be provided.");

            switch (Current)
            {
                case "set":
                    return OperateSetCommands(_Arguments);
                case "auth":
                    return OperateCommands(_Arguments);
                case "file":
                    return OperateCommands(_Arguments);
                case "custom-procedures":
                    return OperateCommands(_Arguments);
                default:
                    return Utilities.Error("Invalid argument. Expected: set or auth or file; given argument is: " + _Arguments.First.Value.ToString());
            }
        }

        private int OperateSetCommands(Arguments _Arguments)
        {
            //version | deployment
            var Invalid_Expected = "Invalid argument. Expected: versionName | deploymentName";

            if (_Arguments.First.Value.ArgumentType != Argument.Type.Binary)
                return Utilities.Error(Invalid_Expected + "; given argument is: " + _Arguments.First.Value.ToString());

            var Cmd = (_Arguments.First.Value as BinaryArgument).Key;
            var Arg = (_Arguments.First.Value as BinaryArgument).Value;

            _Arguments.RemoveFirst();
            if (_Arguments.Count > 0)
            {
                return Utilities.Error("Invalid arguments. Check -help-.");
            }

            switch (Cmd)
            {
                case "versionName":
                    {
                        try
                        {
                            File.WriteAllText(VersionCached_FilePath, Arg);
                        }
                        catch (Exception e)
                        {
                            return Utilities.Error("File write operation has failed with: " + e.Message + ", please check the SDK has access to write/read your documents folder: " + DocumentsFolderPath);
                        }
                        break;
                    }
                case "deploymentName":
                    {
                        try
                        {
                            File.WriteAllText(DeploymentCached_FilePath, Arg);
                        }
                        catch (Exception e)
                        {
                            return Utilities.Error("File write operation has failed with: " + e.Message + ", please check the SDK has access to write/read your documents folder: " + DocumentsFolderPath);
                        }
                        break;
                    }
                default:
                    return Utilities.Error(Invalid_Expected + "; given argument is: " + _Arguments.First.Value.ToString());
            }
            return Utilities.SUCCESS;
        }

        private int OperateCommands(Arguments _Arguments)
        {
            var Invalid_Expected = "Invalid argument. Expected: login | list-users | list-registered-email-addresses | create-user | get-user-info | update-user-info | delete-user | list-access-methods | create-access-method | delete-access-method | list-base-access-rights | upsert-base-access-right  | delete-base-access-right | grant-base-access-rights-to | add-final-access-right | get-final-access-rights | update-final-access-right | delete-final-access-right";

            if (_Arguments.First.Value.ArgumentType != Argument.Type.Unary)
                return Utilities.Error(Invalid_Expected + "; given argument is: " + _Arguments.First.Value.ToString());

            var Current = (_Arguments.First.Value as UnaryArgument).Value;
            _Arguments.RemoveFirst();
            
            Command_0_1 Command = (Command_0_1)Activator.CreateInstance(LoadedCommands[Current], _Arguments);

            return Command.Perform();
        }
    }
}