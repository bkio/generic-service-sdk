/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace SDK.Versions.V_0_1
{
    /// <summary>
    /// sdk_core auth create-user email=\"...\" name=\"...\"
    /// </summary>
    public class Auth_CreateUser : Command_0_1
    {
        public Auth_CreateUser(Arguments _Arguments)

            : base(_Arguments, CheckArguments(out bool bParseable, out int AlternativeIx, _Arguments, new List<List<Argument>>()
            {
                new List<Argument>()
                {
                    new BinaryArgument("email"), new BinaryArgument("name")
                }
            }))

        {
            if (bParseable)
            {
                CreatedRequest = new ApiHttpRequest(BaseApiUrl, "/auth/users").Put(
                    new JObject()
                    {
                        ["userEmail"] = (_Arguments.First.Value as BinaryArgument).Value,
                        ["userName"] = (_Arguments.First.Next.Value as BinaryArgument).Value
                    });
            }
        }

        public override string GetCommandName()
        {
            return "create-user";
        }

        public override List<(int, string)> GetHelpLines()
        {
            return new List<(int, string)>
            {
                (2, "auth create-user email=\"...\" name=\"...\""),
                (0, "\n")
            };
        }
    }
}