/// MIT License, Copyright Burak Kara, burak@burak.io, https://en.wikipedia.org/wiki/MIT_License

using System;
using System.Collections.Generic;

namespace SDK
{
    public class Arguments : LinkedList<Argument>
    {
        public enum EContainsFunctionInput
        {
            UnaryValueOrBinaryKey,
            BinaryValue
        }
        public List<LinkedListNode<Argument>> Contains(Argument.Type _Type, EContainsFunctionInput _What, string _EqualsTo)
        {
            var Result = new List<LinkedListNode<Argument>>();
            if (_Type == Argument.Type.Unary && _What == EContainsFunctionInput.BinaryValue) throw new ArgumentException();

            var CurrentNode = First;
            while (CurrentNode != null)
            {
                if (CurrentNode.Value.ArgumentType == _Type)
                {
                    if ((_Type == Argument.Type.Unary && (CurrentNode.Value as UnaryArgument).Value == _EqualsTo))
                    {
                        Result.Add(CurrentNode);
                    }
                    if (_Type == Argument.Type.Binary && 
                            ((_What == EContainsFunctionInput.UnaryValueOrBinaryKey && (CurrentNode.Value as BinaryArgument).Key == _EqualsTo) 
                            || (_What == EContainsFunctionInput.BinaryValue && (CurrentNode.Value as BinaryArgument).Value == _EqualsTo)))
                    {
                        Result.Add(CurrentNode);
                    }
                }
                CurrentNode = CurrentNode.Next;
            }

            return Result;
        }

        public static int ParseArguments(string[] _Args, out Arguments _Arguments)
        {
            _Arguments = new Arguments();

            if (_Args == null || _Args.Length == 0)
            {
                return Utilities.Error("No argument provided.");
            }

            for (int i = 0; i < _Args.Length; i++)
            {
                var EqArIx = _Args[i].IndexOf("=");
                if (EqArIx == 0) return Utilities.Error("Invalid argument: " + _Args[i]);

                if (EqArIx < 0) _Arguments.AddLast(new UnaryArgument(_Args[i]));
                else if ((EqArIx + 1 /* "=".Length */ ) == _Args[i].Length) return Utilities.Error("Invalid argument: " + _Args[i]);
                else
                {
                    var Key = _Args[i].Substring(0, EqArIx);
                    var ValuesField = _Args[i].Substring(EqArIx + 1 /* "=".Length */).Trim('"');

                    _Arguments.AddLast(new BinaryArgument(Key, ValuesField));
                }
            }

            return Utilities.SUCCESS;
        }

        public override string ToString()
        {
            var Result = "";
            var CurrentNode = First;
            while (CurrentNode != null)
            {
                Result += CurrentNode.Value.ToString() + " ";
                CurrentNode = CurrentNode.Next;
            }
            Result.TrimEnd(' ');
            return Result;
        }
    }

    public abstract class Argument
    {
        public enum Type
        {
            Unary,
            Binary
        }

        public readonly Type ArgumentType;
        protected Argument(Type _Type)
        {
            ArgumentType = _Type;
        }
        private Argument() { }

        public override string ToString()
        {
            return (this is UnaryArgument) ? (this as UnaryArgument).ToString() : (this as BinaryArgument).ToString();
        }
    }

    public class UnaryArgument : Argument
    {
        public readonly string Value;

        public UnaryArgument(string _Value) : base(Type.Unary)
        {
            Value = _Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
    public class BinaryArgument : Argument
    {
        public readonly string Key;
        public readonly string Value;

        public BinaryArgument(string _Key, string _Value = "") : base(Type.Binary)
        {
            Key = _Key;
            Value = _Value;
        }

        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }
}