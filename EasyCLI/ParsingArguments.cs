using EasyCLI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace EasyCLI
{
    public class ParsingArguments
    {

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
                .Select((item) => (obj: item.obj, methods: item.methods.Where((item) => item.name?.ToLower() == name?.ToLower())))
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
            //item.param.All((p) => param.Contains(p.name))
            return methods.Where((item) => item != null)
                .Select((item) => (obj: item.obj,
                                   methods: item.methods.Select((item) => (
                                        method: item.method,
                                        name: item.name,
                                        param: item.method.GetParameters().Select((item) => (p: item, name: GetNameByAttribute(item)))
                                   ))))
                .Select((item) => (obj: item.obj, methods: item.methods.Where((item) => param.All((p) => item.param.Select((item) => item.name.ToLower()).Contains(p.ToLower())))))
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
                    var pf = param.Find((item) => item.name.ToLower() == args[i].Substring(2).ToLower());
                    if (pf != null && i + 1 < args.Length)
                    {
                        pf.value = Type.Missing;
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
            var args = data.Arguments();
            if (args.Length <= 0)
            {
                return null;
            }
            string funcName = null;
            if (args.Length > 1 && !args[1].StartsWith("--"))
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
