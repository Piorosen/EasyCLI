using System;
using System.Collections.Generic;
using System.Linq;
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
            public int index;
        }

        public void AddClass(object item)
        {
            classList.Add(item);
        }

        public void Call(string command)
        {
            var list = ParseArguments(command);
            if (list.Count == 0)
            {
                Console.WriteLine("error");
            }
            var obj = classList.Find((item) => item.GetType().Name.ToLower() == list[0].ToLower());
            var method = obj.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic)
                
            //var param = method.GetParameters()
            //                  .Select((item) => new Param
            //{
            //    name = item.Name,
            //    index = item.Position
            //}).ToList();

            //param.Sort((a, b) => a.index - b.index);

            //for (int i = 2; i < list.Count; i++)
            //{
            //    if (list[i].StartsWith("--"))
            //    {
            //        var p = param.Find((item) => item.name.ToLower() == list[i].Remove(2).ToLower());
            //        i += 1;
            //        p.value = list[i];
            //    }
            //}
            

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
