using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EasyCLI
{
    public class Param
    {
        public string name;
        public object value;
        public Type type;
        public int index;
    }

    public struct Command
    {
        public object ClassObject;

        public MethodInfo FunctionObject;
        public List<Param> Arguments;

#if DEBUG
        public string ClassName;
        public string FunctionName;
#endif
    }

    public class ParsingArguments
    {

        enum QuoteType : int
        {
            SingleQuote,
            DoubleQuote,
            None
        }
        public List<string> SplitArguments(string data)
        {
            List<string> result = new List<string>();

            StringBuilder temp = new StringBuilder();
            QuoteType quote = QuoteType.None;

            for (int i = 0; i < data.Length; i++)
            {
                // Slice Character
                if (data[i] == '\"')
                {
                    if (quote == QuoteType.DoubleQuote)
                    {
                        quote = QuoteType.None;
                        result.Add(temp.ToString());
                        temp.Clear();
                    }
                    else if (quote == QuoteType.SingleQuote)
                    {
                        temp.Append(data[i]);
                    }
                    else if (quote == QuoteType.None)
                    {
                        quote = QuoteType.DoubleQuote;
                    }
                }
                else if (data[i] == '\'')
                {
                    if (quote == QuoteType.SingleQuote)
                    {
                        quote = QuoteType.None;
                        result.Add(temp.ToString());
                        temp.Clear();
                    }
                    else if (quote == QuoteType.DoubleQuote)
                    {
                        temp.Append(data[i]);
                    }
                    else if (quote == QuoteType.None)
                    {
                        quote = QuoteType.SingleQuote;
                    }
                }
                else if (data[i] == ' ' && quote == QuoteType.None)
                {
                    if (temp.Length != 0)
                    {
                        result.Add(temp.ToString());
                        temp.Clear();
                    }
                }
                else
                // Append Character
                if (data[i] == '\\')
                {
                    i += 1;
                    switch (char.ToLower(data[i]))
                    {
                        case 'a':
                            temp.Append('\a');
                            break;
                        case 'b':
                            temp.Append('\b');
                            break;
                        case 'f':
                            temp.Append('\f');
                            break;
                        case 'n':
                            temp.Append('\n');
                            break;
                        case 'r':
                            temp.Append('\r');
                            break;
                        case 't':
                            temp.Append('\t');
                            break;
                        case 'v':
                            temp.Append('\v');
                            break;
                        case '\'':
                            temp.Append('\'');
                            break;
                        case '\"':
                            temp.Append('\"');
                            break;
                        case '\\':
                            temp.Append('\\');
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    temp.Append(data[i]);
                }
            }

            if (temp.Length != 0)
            {
                result.Add(temp.ToString());
            }
            return result;
        }

        public Command? Result(string data, List<object> classList)
        {
            var command = new Command();
            var args = SplitArguments(data);
            if (args.Count <= 0)
            {
                return null;
            }

            var item = classList.Where((item) => item != null)
                .Select((item) => (item, item.GetType()))
                .Select((item) => (item.item, item.Item2.GetCustomAttribute<AlternativeNameAttribute>()?.Name ?? item.Item2.Name))
                .SingleOrDefault((item) => item.Item2.ToLower() == args[0].ToLower());

            command.ClassObject = item.item;

#if DEBUG
            command.ClassName = item.Item2;
#endif

            string funcName = null;
            if (args.Count > 1 && args[1].StartsWith("--"))
            {
                funcName = args[1];
            }

            var obj = item.item;
            var func = obj.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select((item) => (item, item.GetCustomAttribute<AlternativeNameAttribute>()?.Name ?? item.Name))
                .SingleOrDefault((item) => item.Item2?.ToLower() == funcName?.ToLower());

#if DEBUG
            command.FunctionName = func.Item2;
#endif

            var param = func.item.GetParameters().Select((item) => new Param
            {
                name = item.Name,
                index = item.Position,
                type = item.ParameterType,
                value = Type.Missing
            }).OrderBy((item) => item.index)
            .ToList();

            for (int i = 1 + (funcName == null ? 0 : 1); i < args.Count; i++)
            {
                if (args[i].StartsWith("--") && i + 1 < args.Count)
                {
                    var p = param.Find((item) => item.name == args[i].Substring(2));
                    p.value = Convert.ChangeType(args[i + 1], p.type);
                    i += 1;
                }
            }
            command.Arguments = param;

            return command;
        }



    }
}
