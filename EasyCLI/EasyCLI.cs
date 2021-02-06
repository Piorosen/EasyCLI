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
        readonly List<object> classList = new List<object>();
        
        public void AddClass(object item)
        {
            classList.Add(item);
        }

        public bool AddClass(Type type)
        {
            try
            {
                var r = Activator.CreateInstance(type);
                if (r != null)
                {
                    classList.Add(r);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public void RemoveClass(object item)
        {
            classList.Remove(item);
        }

        public void RemoveClass(string name)
        {
            object[] obj = ParsingArguments.FindClassObject(name, classList);
            foreach (var o in obj)
            {
                classList.Remove(o);
            }
        }

        public object Call(string command)
        {
            return Call<object>(command);
        }

        public T Call<T>(string command)
        {
            var list = ParsingArguments.Result(command, classList);

            if (list == null)
            {
                return default;
            }else
            {
                var m = list.Value.FunctionObject;
                var c = list.Value.ClassObject;
                var a = list.Value.Arguments;
                return (T)m.Invoke(c, a.Select((a) => a.value).ToArray());
            }
        }
    }
}
