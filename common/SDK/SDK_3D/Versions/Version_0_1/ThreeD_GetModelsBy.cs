/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using System.Net;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// 3d get-models-by [userId=\"...\"] metadataKey=\"...\" [metadataValues=\"...\"]
    /// </summary>
    public class ThreeD_GetModelsBy : Command_0_1
    {
        public ThreeD_GetModelsBy(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("metadataKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("metadataKey"), new BinaryArgument("metadataValues")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("metadataKey")
                },
                new List<Argument>()
                {
                    new BinaryArgument("userId"), new BinaryArgument("metadataKey"), new BinaryArgument("metadataValues")
                }
            }))

        {
            if (bParseable)
            {
                string UserID;
                if (AlternativeIx == 0 || AlternativeIx == 1)
                {
                    if (UserIDCached == null)
                    {
                        Utilities.Error("You must login to have the userId to be cached and used in the requests.");
                        bErrorOccuredInChildConstructor = true;
                        return;
                    }
                    UserID = UserIDCached;
                }
                else
                {
                    UserID = (_Arguments.First.Value as BinaryArgument).Value;
                    _Arguments.RemoveFirst();
                }

                var MetadataKeyUrlEncoded = WebUtility.UrlEncode((_Arguments.First.Value as BinaryArgument).Value);
                _Arguments.RemoveFirst();

                if (AlternativeIx == 1 || AlternativeIx == 3)
                {
                    var MetadataValuesUrlEncoded = WebUtility.UrlEncode((_Arguments.First.Value as BinaryArgument).Value);
                    CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/3d/models/get_models_by/user_id/" + UserID + "/metadata_key/" + MetadataKeyUrlEncoded + "/metadata_values/" + MetadataValuesUrlEncoded).Get();
                }
                else
                {
                    CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/3d/models/get_models_by/user_id/" + UserID + "/metadata_key/" + MetadataKeyUrlEncoded).Get();
                }
            }
        }

        public override string GetCommandName()
        {
            return "get-models-by";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "3d get-models-by [userId=\"...\"] metadataKey=\"...\" [metadataValues=\"...\"]"),
                (0, "\tmetadataValues: Values should be separated by __DELIM__ For example; MyValue1__DELIM__MyOtherValue 2__DELIM__Third one."),
                (0, "\n")
            };
        }
    }
}