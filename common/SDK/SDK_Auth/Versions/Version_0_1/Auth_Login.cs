/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// auth login sso
    /// auth login apiKey=\"...\"
    /// auth login email=\"...\" passwordMd5=\"...\"
    /// auth login name=\"...\" passwordMd5=\"...\"
    /// </summary>
    public class Auth_Login:Command_0_1
    {
        private const string EmailRegexVal = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        public enum ELoginType
        {
            None,
            SSO,
            EmailPasswordMD5,
            UsernamePasswordMD5,
            ApiKey
        }

        private static bool Check(Arguments _Arguments, out bool _bArgsParseableAndCorrect, out ELoginType _LoginType)
        {
            _LoginType = ELoginType.None;

            if (_Arguments.Count == 1)
            {
                if (_Arguments.First.Value is UnaryArgument && (_Arguments.First.Value as UnaryArgument).Value == "sso")
                {
                    _LoginType = ELoginType.SSO;
                    return _bArgsParseableAndCorrect = true;
                }
                if (_Arguments.First.Value is BinaryArgument && (_Arguments.First.Value as BinaryArgument).Key == "apiKey")
                {
                    _LoginType = ELoginType.ApiKey;
                    return _bArgsParseableAndCorrect = true;
                }
                return _bArgsParseableAndCorrect = false;
            }

            if (_Arguments.Count == 2
                && _Arguments.First.Value is BinaryArgument
                && _Arguments.First.Next.Value is BinaryArgument)
            {
                var Second = _Arguments.First.Next.Value as BinaryArgument;
                if (Second.Key != "passwordMd5") return _bArgsParseableAndCorrect = false;

                var First = _Arguments.First.Value as BinaryArgument;
                if (!Regex.IsMatch(First.Value, EmailRegexVal, RegexOptions.IgnoreCase)) return _bArgsParseableAndCorrect = false;

                if (First.Key == "email")
                {
                    _LoginType = ELoginType.EmailPasswordMD5;
                    return _bArgsParseableAndCorrect = true;
                }
                if (First.Key == "name")
                {
                    _LoginType = ELoginType.UsernamePasswordMD5;
                    return _bArgsParseableAndCorrect = true;
                }
            }

            return _bArgsParseableAndCorrect = false;
        }

        private readonly Arguments Args;

        public Auth_Login(Arguments _Arguments) :base(_Arguments, false)
        {
            Args = _Arguments;
        }

        public override int Perform()
        {
            if (!Check(Args, out bool _, out ELoginType LoginType))
            {
                return Utilities.Error("Invalid arguments. Use -help- to see commands and valid arguments. Arguments provided: " + Args.ToString());
            }

            if (LoginType == ELoginType.SSO)
            {
                return new Auth_LoginSSO(Args).Perform();
            }
            return new Auth_LoginOther(Args, LoginType).Perform();
        }

        public override string GetCommandName()
        {
            return "login";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>{
                (2, "auth login sso"),
                (2, "auth login apiKey=\"...\""),
                (2, "auth login email=\"...\" passwordMd5=\"...\""),
                (2, "auth login name=\"...\" passwordMd5=\"...\""),
                (0, "\n")
            };
        }
    }
}