﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
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

    public class Method
    {
        public String name;
        public MethodInfo method;
    }

    public class ObjectMethods
    {
        public object obj;
        public Method[] methods;

    }

    public struct Command
    {
        public object ClassObject;

        public MethodInfo FunctionObject;
        public Param[] Arguments;
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

        public string GetNameByAttribute(object obj)
        {
            AlternativeNameAttribute attr;
            var type = obj.GetType();

            string typeName;
            switch (obj)
            {
                case MethodInfo m:
                    attr = m.GetCustomAttribute<AlternativeNameAttribute>();
                    typeName = m.Name;
                    break;
                case ParameterInfo p:
                    attr = p.GetCustomAttribute<AlternativeNameAttribute>();
                    typeName = p.Name;
                    break;
                default:
                    attr = type.GetCustomAttribute<AlternativeNameAttribute>();
                    typeName = type.Name;
                    break;
            }

            return attr != null ? attr.Name : typeName;
        }

        public object[] FindClassObject(string name, List<object> objectList)
        {
            return objectList.Where((item) => item != null)
                .Select((item) => (item, GetNameByAttribute(item)))
                .Where((obj) => obj.Item2.ToLower() == name.ToLower())
                .Select((obj) => obj.item)
                .ToArray();
        }

        public ObjectMethods[] FindMethodObject(string name, object[] objectList)
        {
            return objectList.Where((item) => item != null)
                .Select((item) => (obj: item, methods: item.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)))
                .Select((item) => (obj: item.obj, methods: (item.methods.Select((item) => (method: item, name: GetNameByAttribute(item))))))
                .Select((item) => (obj: item.obj, methods: item.methods.Where((item) => item.name.ToLower() == name.ToLower())))
                .Select((item) => new ObjectMethods
                {
                    obj = item.obj,
                    methods = item.methods.Select((item) => new Method
                    {
                        method = item.method,
                        name = item.name
                    }).ToArray()
                })
                .ToArray();
        }

        public ObjectMethods[] FilterMethodObjectByParameterName(string[] param, ObjectMethods[] methods)
        {
            return methods.Where((item) => item != null)
                .Select((item) => (obj: item.obj,
                                   methods: item.methods.Select((item) => (
                                        method: item.method,
                                        name: item.name,
                                        param: item.method.GetParameters().Select((item) => (p: item, name: GetNameByAttribute(item)))
                                   ))))
                .Select((item) => (obj: item.obj, methods: item.methods.Where((item) => item.param.All((p) => param.Contains(p.name)))))
                .Select((item) => new ObjectMethods
                {
                    obj = item.obj,
                    methods = item.methods.Select((item) => new Method
                    {
                        method = item.method,
                        name = item.name
                    }).ToArray(),
                })
                .ToArray();
        }

        public Param[] GetParametersFromMethodInfo(MethodInfo method, string[] args)
        {
            var param = method.GetParameters()
                .Select((item) => (param: item, name: GetNameByAttribute(item)))
                .Select((item) => new Param
                {
                    name = item.name,
                    index = item.param.Position,
                    type = item.param.ParameterType,
                    value = Type.Missing
                }).OrderBy((item) => item.index)
            .ToList();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var pf = param.Find((item) => item.name == args[i].Substring(2));
                    if (pf != null && i + 1 < args.Length)
                    {
                        pf.value = default;
                        if (!args[i + 1].StartsWith("--"))
                        {
                            pf.value = Convert.ChangeType(args[i + 1], pf.type);
                        }
                    }
                }
            }

            return param.ToArray();
        }

        public Command? Result(string data, List<object> classList)
        {
            var args = SplitArguments(data);
            if (args.Count <= 0)
            {
                return null;
            }
            string funcName = null;
            if (args.Count > 1 && !args[1].StartsWith("--"))
            {
                funcName = args[1];
            }

            var objList = FindClassObject(args[0], classList);
            var funcList = FindMethodObject(funcName, objList);
            var argsList = args.Skip(1 + funcName == null ? 0 : 1)
                .Where((item) => item.StartsWith("--"))
                .Select((item) => item.Substring(2))
                .ToArray();

            var equalParam = FilterMethodObjectByParameterName(argsList, funcList);
            if (equalParam.Length != 1)
            {
                throw new Exception("일치하는 클래스의 종류가 2개 이상이거나 없습니다.");
            }
            if (equalParam[0].methods.Length != 1)
            {
                throw new Exception("일치하는 클래스가 1개 이지만 일치하는 메소드가 2개 이상이거나 없습니다.");
            }

            // 개수 확인 했으므로 Single, 객체 추출 가능함.
            var command = new Command
            {
                ClassObject = equalParam.Single().obj,
                FunctionObject = equalParam.First().methods.First().method,
                Arguments = GetParametersFromMethodInfo(equalParam.Single().methods.Single().method,
                                                        args.Skip(1 + funcName == null ? 0 : 1)
                                                        .ToArray())
            };



            return command;
        }
    }
}
