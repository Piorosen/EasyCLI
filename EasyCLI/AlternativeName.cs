using System;
namespace EasyCLI
{
    /// <summary>
    /// EasyCLI와 함께 동작하며, 명령어에서 이름의 대체제로 사용이 됩니다. <br />
    /// 1. Class Name<br />
    /// 2. Method Name<br />
    /// 3. Paramerter Name<br />
    /// 위의 조건만 사용 가능합니다.
    /// </summary>
    public class AlternativeNameAttribute : Attribute
    {
        /// <summary>
        /// 객체의 대체할 이름 입니다.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 생성자 입니다.
        /// </summary>
        /// <param name="name">객체의 이름을 대체할 이름 입니다.</param>
        public AlternativeNameAttribute(string name = null)
        {
            Name = name;
        }
    }
}
