/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.IO;
using SDK.Versions;

namespace SDK
{
    public abstract class Version
    {
        public enum EVersion
        {
            V_0_1
        }
        public readonly static Dictionary<string, EVersion> StringToEnum = new Dictionary<string, EVersion>()
        {
            ["0.1"] = EVersion.V_0_1
        };
        public readonly static Dictionary<EVersion, string> EnumToString = new Dictionary<EVersion, string>()
        {
            [EVersion.V_0_1] = "0.1"
        };
        private readonly static Dictionary<EVersion, Func<Version>> EnumToInitializer = new Dictionary<EVersion, Func<Version>>()
        {
            [EVersion.V_0_1] = () => { return new Version_0_1(); }
        };
        public const EVersion LATEST_VERSION = EVersion.V_0_1;

        public readonly EVersion SDKVersion;

        public static int InitializeVersion(Arguments _Arguments, string _DeploymentName, out Version _Version, out string _BaseApiUrl)
        {
            _Version = null;
            _BaseApiUrl = null;

            if (GetAndSetSettingsAndCachedInfo() == Utilities.FAILURE) return Utilities.FAILURE;

            var VAs = _Arguments.Contains(Argument.Type.Binary, Arguments.EContainsFunctionInput.UnaryValueOrBinaryKey, "version");

            if (VAs.Count > 1) return Utilities.Error("Multiple version arguments cannot be provided.");

            if (VAs.Count == 0)
            {
                if (Version_SetExclusivelyInPreviousSession != null)
                {
                    if (!StringToEnum.TryGetValue(Version_SetExclusivelyInPreviousSession, out EVersion _EnumFromCached))
                    {
                        try
                        {
                            File.Delete(VersionCached_FilePath);
                        }
                        catch (Exception) { }
                        return Utilities.Error("Invalid cached version argument. Please try again.");
                    }
                    _Version = EnumToInitializer[_EnumFromCached]();
                }
                else _Version = EnumToInitializer[LATEST_VERSION]();

                if (_Version.DeploymentToApiBaseUrl(_DeploymentName, out _BaseApiUrl) == Utilities.FAILURE) return Utilities.FAILURE;
                return Utilities.SUCCESS;
            }

            var RequestedVersion = (VAs[0].Value as BinaryArgument).Value;
            if (!StringToEnum.TryGetValue(RequestedVersion, out EVersion _Enum)) return Utilities.Error("Invalid version argument. Available versions: " + string.Join(", ", StringToEnum.Keys));
            
            _Version = EnumToInitializer[_Enum]();
            if (_Version.DeploymentToApiBaseUrl(_DeploymentName, out _BaseApiUrl) == Utilities.FAILURE) return Utilities.FAILURE;

            return Utilities.SUCCESS;
        }

        protected Version(EVersion _Version)
        {
            SDKVersion = _Version;
        }

        protected static string BaseApiUrl;

        protected static string DocumentsFolderPath;
        protected static string UserIDFilePath;
        protected static string UserIDCached;
        protected static string TokenFilePath;
        protected static string TokenCached;

        protected static string Version_SetExclusivelyInPreviousSession = null;
        protected static string Deployment_SetExclusivelyInPreviousSession = null;

        protected static string VersionCached_FilePath;
        protected static string DeploymentCached_FilePath;

        public int Operate(Arguments _Arguments, string _BaseApiUrl)
        {
            BaseApiUrl = _BaseApiUrl;

            var CleanLineSeparator = "____________________________";

            var CurrentArgumentNode = _Arguments.First;
            if (CurrentArgumentNode.Value is UnaryArgument && (CurrentArgumentNode.Value as UnaryArgument).Value.Equals("help", StringComparison.InvariantCultureIgnoreCase))
            {
                if (CurrentArgumentNode.Next != null) return Utilities.Error("First argument is -help- but more arguments provided after.");

                var Merged = "\n" + CleanLineSeparator + "\n\nVersion " + EnumToString[SDKVersion] + " guide:\n" + CleanLineSeparator + "\n\n\n\n";
                var HelpLines = GetHelpLines();
                foreach (var Line in HelpLines)
                {
                    if (Line.Item1 == 0) Merged += Line.Item2 + "\n";
                    else if (Line.Item1 == 1) Merged += CleanLineSeparator;
                    else if (Line.Item1 == 2) Merged += Utilities.ASSEMBLY_NAME + " " + Line.Item2 + "\n";
                }

                Utilities.Print(Merged);
                return Utilities.SUCCESS_NO_JSON_OUTPUT_EXCEPTION;
            }
            return OperateVersionSpecific(_Arguments);
        }

        private static int GetAndSetSettingsAndCachedInfo()
        {
            try
            {
                DocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd(Path.DirectorySeparatorChar);
                UserIDFilePath = DocumentsFolderPath + Path.DirectorySeparatorChar + "bkio.user.id";
                TokenFilePath = DocumentsFolderPath + Path.DirectorySeparatorChar + "bkio.user.token";
                VersionCached_FilePath = DocumentsFolderPath + Path.DirectorySeparatorChar + "bkio.preferred.version";
                DeploymentCached_FilePath = DocumentsFolderPath + Path.DirectorySeparatorChar + "bkio.preferred.deployment";
            }
            catch (Exception e)
            {
                return Utilities.Error("File get operation has failed with: " + e.Message + ", please check the SDK has access to write/read your documents folder: " + DocumentsFolderPath);
            }

            try
            {
                UserIDCached = File.ReadAllText(UserIDFilePath);
                TokenCached = File.ReadAllText(TokenFilePath);
            }
            catch (Exception e)
            {
                if (!(e is FileNotFoundException))
                {
                    return Utilities.Error("File read operation has failed with: " + e.Message + ", please check the SDK has access to write/read your documents folder: " + DocumentsFolderPath);
                }
                UserIDCached = null;
                TokenCached = null;
            }

            try
            {
                Version_SetExclusivelyInPreviousSession = File.ReadAllText(VersionCached_FilePath);
            }
            catch (Exception e)
            {
                if (!(e is FileNotFoundException))
                {
                    return Utilities.Error("File read operation has failed with: " + e.Message + ", please check the SDK has access to write/read your documents folder: " + DocumentsFolderPath);
                }
                Version_SetExclusivelyInPreviousSession = null;
            }

            try
            {
                Deployment_SetExclusivelyInPreviousSession = File.ReadAllText(DeploymentCached_FilePath);
            }
            catch (Exception e)
            {
                if (!(e is FileNotFoundException))
                {
                    return Utilities.Error("File read operation has failed with: " + e.Message + ", please check the SDK has access to write/read your documents folder: " + DocumentsFolderPath);
                }
                Deployment_SetExclusivelyInPreviousSession = null;
            }

            return Utilities.SUCCESS;
        }

        protected abstract List<ValueTuple<int, string>> GetHelpLines();
        protected abstract int DeploymentToApiBaseUrl(string _Deployment, out string _BaseApiUrl);
        protected abstract int OperateVersionSpecific(Arguments _Arguments);
    }
}