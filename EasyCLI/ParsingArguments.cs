using EasyCLI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace EasyCLI
{
    static class ParsingArguments
    {
        static string ConvertString(string value)
        {
            return Option.Instance.IsPerfectNameCheck ? value : value?.ToLower();
        }
        static BindingFlags BindingFlags => Option.Instance.BindingFlags;

        static string GetNameByAttribute(object obj)
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

        /// <summary>
        /// Class 이름(타입) 기준으로 일치하는 객체를 찾아 반환 합니다. <br />
        /// (네임스페이스는 구별하지 않으며, AlternativeName으로 이름이 중복이 될 가능성이 있으므로 배열로 반환합니다.)
        /// </summary>
        /// <param name="name">Class의 이름(타입)</param>
        /// <param name="objectList">찾을 Object의 목록</param>
        /// <returns>이름(타입)이 같은 객체</returns>
        public static object[] FindClassObject(string name, List<object> objectList)
        {
            // objectList 목록에 null 이 있는지 확인합니다.
            return objectList.Where((item) => item != null)
                // 타입을 (obj, TypeName)으로 변경합니다.
                .Select((item) => (item, GetNameByAttribute(item)))
                // 찾을 이름과 TypeName이 일치하는 객체만 찾습니다.
                .Where((obj) => ConvertString(obj.Item2) == ConvertString(name))
                // 타입을 obj 배열로 변경합니다.
                .Select((obj) => obj.item)
                .ToArray();
        }

        /// <summary>
        /// Method 이름 (선언 또는 호출 명) 기준으로 일치하는 함수만 찾아 반환 합니다. <br />
        /// 객체를 반환 하는 이유는 호출 할 때 필요하기 때문입니다. <br />
        /// (네임스페이스는 구별하지 않으며, AlternativeName으로 이름이 중복이 될 가능성이 있으므로 배열로 반환합니다.)
        /// </summary>
        /// <param name="name">Method 이름 또는 선언 명</param>
        /// <param name="objectList">찾을 Object의 목록</param>
        /// <returns>이름(선언 명)이 같은 함수를 가진 객체</returns>
        public static ObjectMethods[] FindMethodObject(string name, object[] objectList)
        {
            // Null 체크
            return objectList.Where((item) => item != null)
                // (객체, [함수 목록])
                .Select((item) => (obj: item, methods: item.GetType().GetMethods(BindingFlags)))
                // (객체, (함수 정보, 함수 이름)
                .Select((item) => (obj: item.obj, methods: (item.methods.Select((item) => (method: item, name: GetNameByAttribute(item))))))
                // 함수 이름이 같은 객체만 탐색.
                .Select((item) => (obj: item.obj, methods: item.methods.Where((item) => ConvertString(item.name) == ConvertString(name))))
                // ObjectMethods[] 타입으로 변경함.
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

        /// <summary>
        /// Method[] 를 입력 받으며, 해당 함수들 목록에서 Parmater명 기준으로 이름이 같은지 확인 한 후, 반환 합니다. <br />
        /// 즉 함수 목록을 필터링 합니다.
        /// </summary>
        /// <param name="param">파라미터 이름(선언 명)을 받습니다.</param>
        /// <param name="methods">메소드 목록을 받습니다.</param>
        /// <returns>파라미터 명이 같은 메소드만 반환 합니다.</returns>
        public static ObjectMethods[] FilterMethodObjectByParameterName(string[] param, ObjectMethods[] methods)
        {
            // Null 체크
            return methods.Where((item) => item != null)
                // 타입 변경. ObjectMethods에서는 Parameters에 관한 코드가 없으므로 아래 처럼 변경 함.
                .Select((item) => (obj: item.obj,
                                   methods: item.methods.Select((item) => (
                                        method: item.method,
                                        name: item.name,
                                        param: item.method.GetParameters().Select((item) => (p: item, name: GetNameByAttribute(item)))
                                   ))))
                // Param(args)의 이름이 Methods안에 있는 (args)와 이름이 모두 포함이 되어야 합니다.
                // Param.All((p) => method.param.Contains(p) 예시.
                .Select((item) => (obj: item.obj, methods: item.methods.Where((item) => param.All((p) => item.param.Select((item) => ConvertString(item.name)).Contains(ConvertString(p))))))
                // 필터링을 한 후, 결과를 ObjectMethods로 변경하여 반환 합니다.
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

        /// <summary>
        /// MethodInfo에 있는 Parameter정보를 가지고 온 후, args를 파싱하여 Param으로 반환 합니다. <br />
        /// Param안에는 Parameter의 타입, 값, 파라미터 순서, 파라미터 이름이 담겨져 있습니다.
        /// </summary>
        /// <param name="method">Method 입니다.</param>
        /// <param name="args">ParsingStringExtension에서 나온 결과를 입력 받습니다.</param>
        /// <returns>Parameter를 반환 합니다.</returns>
        public static Param[] GetParametersFromMethodInfo(MethodInfo method, string[] args)
        {
            // Method의 파라미터를 가지고 옵니다.
            var param = method.GetParameters()
                // 파라미터와 파라미터 명을 가지고 옵니다.
                // 파라미터 명을 별도로 분리하는 이유는 Attribute와 연계가 되어 있기 때문에
                // 바로 사용이 불가능하기 때문입니다.
                .Select((item) => (param: item, name: GetNameByAttribute(item)))
                // ParameterInfo를 Param으로 변경합니다.
                .Select((item) => new Param
                {
                    name = item.name,
                    index = item.param.Position,
                    type = item.param.ParameterType,
                    value = Type.Missing
                })
                // 순서를 index로 정렬 합니다.
                .OrderBy((item) => item.index)
                // ToArray()가 아닌 이유는 밑에 Find, 즉 레퍼런스로 참조하기 위함 입니다.
                .ToList();

            // 처음부터 반복문으로 돌며, "--"를 찾을 경우 다음 글자가 마지막 요소가 아니어야 합니다.
            // 그리고 다음 요소가 "--"을 가질 경우 파라미터 명이므로 무시하도록 합니다.
            // 사용자가 "--" 를 입력하고 싶다면 \"--\" 로 입력을 해야 합니다.
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var pf = param.Find((item) => ConvertString(item.name) == ConvertString(args[i].Substring(2)));
                    // null을 체크 합니다. (예외 처리)
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

        /// <summary>
        /// ParsingArguments의 주 핵심 기능입니다. <br />
        /// Arguments에 문제가 있다면 결과를 Null을 반환을 하며, <br />
        /// 그 외, 개발자의 실수 인 경우에는 Throw Exception을 발생 시킵니다.
        /// </summary>
        /// <param name="data">사용자의 입력을 받습니다.</param>
        /// <param name="classList">개발자가 등록한 클래스 목록을 받습니다.</param>
        /// <returns></returns>
        public static Command? Result(string data, List<object> classList)
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
            var argsList = args.Skip(1 + funcName == null ? 0 : 1);
            var findArgsList = args.Where((item) => item.StartsWith("--"))
                                   .Select((item) => item.Substring(2))
                                   .ToArray();


            var equalParam = FilterMethodObjectByParameterName(findArgsList, funcList);
            if (equalParam.Length > 1)
            {
                throw new Exception("일치하는 클래스의 종류가 2개 이상이거나 없습니다.");
            }
            else if (equalParam.Length == 0)
            {
                return null;
            }
            if (equalParam[0].methods.Length > 1)
            {
                throw new Exception("일치하는 클래스가 1개 이지만 일치하는 메소드가 2개 이상이거나 없습니다.");
            }
            else if (equalParam[0].methods.Length == 0) 
            {
                return null;
            }

            // 개수 확인 했으므로 Single, 객체 추출 가능함.
            var command = new Command
            {
                ClassObject = equalParam.Single().obj,
                FunctionObject = equalParam.First().methods.First().method,
                Arguments = GetParametersFromMethodInfo(equalParam.Single().methods.Single().method,
                                                        args.ToArray())
            };

            return command;
        }
    }
}
