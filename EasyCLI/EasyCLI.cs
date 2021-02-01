using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCLI
{
    public class EasyCLI
    {
        enum QuoteType : int
        {
            SingleQuote,
            DoubleQuote,
            None
        }

        public void AddClass(object item)
        {

        }

        public List<String> ParseArguments(string data)
        {
            List<String> result = new List<string>();

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
