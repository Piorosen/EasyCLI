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

        public T Call<T>(string command)
        {
            var list = new ParsingArguments().Result(command, classList);

            if (list == null)
            {
                return default;
            }else
            {
                return (T)list.Value.FunctionObject.Invoke(list.Value.ClassObject,
                                                           list.Value.Arguments.OrderBy((item) => item.index)
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
