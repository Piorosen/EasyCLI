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

        /// <summary>
        /// EasyCLI를 동작하는데 필요한 설정 입니다.
        /// </summary>
        public Option Option { get; private set; } = Option.Instance;

        #region Class 추가 및 삭제 기능
        /// <summary>
        /// 객체를 추가 합니다.
        /// </summary>
        /// <param name="item">command에 명령어를 추가 합니다.</param>
        public void AddClass(object item)
        {
            classList.Add(item);
        }

        /// <summary>
        /// 디폴트 생성자를 이용하여 객체를 추가 합니다. <br />
        /// 실패할 경우 false, 성공적으로 추가할 경우 true을 반환 합니다.
        /// </summary>
        /// <param name="type">command에 명령어를 추가 합니다.</param>
        /// <returns>데이터 삽입의 성공 여부 입니다.</returns>
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

        /// <summary>
        /// 객체를 삭제 합니다.
        /// </summary>
        /// <param name="item">객체를 입력 받습니다.</param>
        public void RemoveClass(object item)
        {
            classList.Remove(item);
        }
        
        /// <summary>
        /// 객체의 타입명을 기준으로 객체를 삭제 합니다.
        /// </summary>
        /// <param name="name">타입의 이름을 받습니다.</param>
        public void RemoveClass(string name)
        {
            object[] obj = ParsingArguments.FindClassObject(name, classList);
            foreach (var o in obj)
            {
                classList.Remove(o);
            }
        }
        #endregion

        /// <summary>
        /// Call<object>(command) 로 동작합니다.
        /// Example) Console WriteLine --value \"hello\"
        /// 극단적인 예시 입니다.
        /// </summary>
        /// <param name="command">명령어를 수행할 커맨드 입니다.</param>
        /// <returns>Object를 반환 합니다.</returns>
        public object Call(string command)
        {
            return Call<object>(command);
        }

        /// <summary>
        /// Example) Console WriteLine --value \"hello\"
        /// 극단적인 예시 입니다.
        /// </summary>
        /// <typeparam name="T">반환할 타입입니다.</typeparam>
        /// <param name="command">명령어를 수행할 커맨드 입니다.</param>
        /// <returns>T를 반환 합니다.</returns>
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
