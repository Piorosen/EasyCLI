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
                                  type = item.ParameterType,
                                  value = Type.Missing
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
            var list = new ParsingArguments().Result(command, classList);

            if (list == null)
            {
                return default;
            }else
            {
                return (T)list.Value.FunctionObject.Invoke(list.Value.ClassObject, list.Value.Arguments.OrderBy((item) => item.index)
                    .Select((item) => item.value)
                    .ToArray());
            }
            //if (list.Count == 0)
            //{
            //    Console.WriteLine("error");
            //    return default;
            //}

            //var obj = FindClassObject(list[0]);
            //if (obj == null)
            //{
            //    Console.WriteLine("data is null");
            //    return default;
            //}

            //var method = FindMethod(obj, list[1]);
            //if (method == null)
            //{
            //    Console.WriteLine("method is null");
            //    return default;
            //}


            //var param = GetParameter(method);
        }


    }
}
