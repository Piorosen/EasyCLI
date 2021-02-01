using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace EasyCLI
{
    public class EasyCLI
    {
        List<object> classList = new List<object>();

        class Param
        {
            public string name;
            public object value;
            public Type type;
            public int index;
        }

        public void AddClass(object item)
        {
            classList.Add(item);
        }

        public object Call(string command)
        {
            return Call<object>(command);
        }

        List<Param> GetParameter(MethodInfo method)
        {
            return method.GetParameters()
                              .Select((item) => new Param
                              {
                                  name = item.Name,
                                  index = item.Position,
                                  type = item.ParameterType
                              })
                              .OrderBy((item) => item.index)
                              .ToList();
        }
        MethodInfo FindMethod(object obj, string name)
        {
            var method = obj.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault((item) => item.Name.ToLower() == name.ToLower());
            return method;
        }

        object[] SetParamAndArgument(List<Param> param, List<string> args)
        {
            if (args.Count % 2 != 0)
            {
                return param.Select((item) => item.value).ToArray();
            }

            for (int i = 2; i < args.Count; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var e = args[i].Substring(2).ToLower();
                    var p = param.Find((item) => item.name.ToLower() == e);
                    i += 1;
                    var temp = Convert.ChangeType(args[i], p.type);
                    var data = temp == null ? Type.Missing : temp;

                    p.value = data;
                }
            }

            return param.Select((item) => item.value).ToArray();
        }

        object FindClassObject(string name)
        {
            return classList.Find((item) => item.GetType().Name.ToLower() == name.ToLower());
        }

        public T Call<T>(string command)
        {
            var list = ParseArguments(command);
            if (list.Count == 0)
            {
                Console.WriteLine("error");
                return default;
            }

            var obj = FindClassObject(list[0]);
            if (obj == null)
            {
                Console.WriteLine("data is null");
                return default;
            }

            var method = FindMethod(obj, list[1]);
            if (method == null)
            {
                Console.WriteLine("method is null");
                return default;
            }

            var param = GetParameter(method);
            SetParamAndArgument(param, list);

            var eeeppparm = ppparm.Select((item) => )
                .Select((item) => item == null ? Type.Missing : item).ToArray();

            return (T)method.Invoke(obj, eeeppparm);
        }


        enum QuoteType : int
        {
            SingleQuote,
            DoubleQuote,
            None
        }
        public List<string> ParseArguments(string data)
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
                else  if (data[i] == '\'')
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
    }
}
