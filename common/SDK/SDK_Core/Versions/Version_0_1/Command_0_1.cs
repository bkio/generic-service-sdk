/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SDK.Versions
{
    public abstract class Command_0_1
    {
        protected bool bErrorOccuredInChildConstructor = false;

        private readonly bool bArgsParseableAndCorrect;

        public abstract string GetCommandName();

        public abstract List<ValueTuple<int, string>> GetHelpLines();

        protected static bool CheckArguments(
            out bool _bArgumentsParseable,
            out int _FoundTemplateAlternativeIx,
            Arguments _Arguments,
            List<List<Argument>> _TemplateAlternatives,
            bool _bCheckUnaryMatch = true)
        {
            _FoundTemplateAlternativeIx = -1;

            if (_Arguments.Count == 0 && _TemplateAlternatives.Count == 0) return _bArgumentsParseable = true;

            int TemplateIx = 0;
            foreach (var TemplateAlternative in _TemplateAlternatives)
            {
                if (_Arguments.Count == TemplateAlternative.Count)
                {
                    bool bMatch = true;
                    var CurrentNode = _Arguments.First;
                    foreach (var TemplateCurrent in TemplateAlternative)
                    {
                        if (CurrentNode == null
                            || CurrentNode.Value.ArgumentType != TemplateCurrent.ArgumentType
                            || (CurrentNode.Value.ArgumentType == Argument.Type.Binary && (CurrentNode.Value as BinaryArgument).Key != (TemplateCurrent as BinaryArgument).Key)
                            || (_bCheckUnaryMatch && CurrentNode.Value.ArgumentType == Argument.Type.Unary && (CurrentNode.Value as UnaryArgument).Value != (TemplateCurrent as UnaryArgument).Value))
                        {
                            bMatch = false;
                            break;
                        }

                        CurrentNode = CurrentNode.Next;
                    }
                    if (bMatch)
                    {
                        if (CurrentNode == null)
                        {
                            _FoundTemplateAlternativeIx = TemplateIx;
                            return _bArgumentsParseable = true;
                        }
                        return _bArgumentsParseable = false;
                    }
                }
                TemplateIx++;
            }
            return _bArgumentsParseable = false;
        }

        protected ApiHttpRequest CreatedRequest;

        private readonly Arguments Args;

        public Command_0_1(Arguments _Args, bool _bArgsParseableAndCorrect = false)
        {
            Args = _Args;
            bArgsParseableAndCorrect = _bArgsParseableAndCorrect;
        }

        public static string BaseApiUrl;
        public static string UserIDCached;

        public static string DocumentsFolderPath;
        public static string UserIDFilePath;
        public static string TokenFilePath;

        public virtual int Perform()
        {
            if (!bArgsParseableAndCorrect) return Utilities.Error("Invalid arguments. Use -help- to see commands and valid arguments. Arguments provided: " + Args.ToString());
            if (bErrorOccuredInChildConstructor) return Utilities.FAILURE;

            if (CreatedRequest != null)
            {
                if (CreatedRequest.Perform(out int _ResultHttpCode, out JObject _ResultJson) == Utilities.FAILURE) return Utilities.FAILURE;
                return Perform_Internal_WithHttpRequest(_ResultHttpCode, _ResultJson);
            }

            return Perform_Internal_WithoutHttpRequest();
        }

        protected virtual int Perform_Internal_WithoutHttpRequest() { return Utilities.SUCCESS; }
        protected virtual int Perform_Internal_WithHttpRequest(int _ResultHttpCode, JObject _ResultJson)
        {
            var Stringified = _ResultJson.ToString(Formatting.Indented);
            return _ResultHttpCode < 400 ? Utilities.Success(Stringified) : Utilities.Error(Stringified);
        }
    }

}