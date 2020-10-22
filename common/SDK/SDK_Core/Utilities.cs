/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SDK
{
    public static class Utilities
    {
        //Constants
        public const int FAILURE = 1;
        public const int SUCCESS = 0;
        public const int SUCCESS_NO_JSON_OUTPUT_EXCEPTION = -1;
        public const string ASSEMBLY_NAME = "sdk_core";

        public static readonly List<string> Messages = new List<string>();

        public static readonly Func<string, int> Error = (string _Message) => { Messages.Add(_Message); return FAILURE; };
        public static readonly Func<string, int> Success = (string _Message) => { Messages.Add(_Message); return SUCCESS; };
        public static readonly Action<string> Warning = (string _Message) => { Messages.Add(_Message); };
        public static readonly Action<string> Log = (string _Message) => { Messages.Add(_Message); };

        //Output configuration, no Console.Writeline used in the other classes
        public static readonly Action<string> Print = (string _Message) => { Console.WriteLine(_Message); };


        public static string GenerateMD5(string _Input)
        {
            var Md5 = MD5.Create();
            var InputBytes = Encoding.UTF8.GetBytes(_Input);
            var Hash = Md5.ComputeHash(InputBytes);
            var Builder = new StringBuilder();
            for (int i = 0; i < Hash.Length; i++)
            {
                Builder.Append(Hash[i].ToString("x2"));
            }
            return Builder.ToString().ToLower();
        }
    }
}