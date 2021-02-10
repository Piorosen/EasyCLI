using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EasyCLI
{
    public sealed class Option
    {
        private static Option _singleton = null;
        private static object _lock = new object();

        public static Option Instance
        {
            get
            {
                if (_singleton == null)
                {
                    lock(_lock)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new Option();
                        }
                    }
                }

                return _singleton;
            }
        }

        /// <summary>
        /// 클래스 명, 함수 목록을 가져 올 때, 어떤 조건으로 가져 올지 설정합니다. <br />
        /// 기본값 : Public | NonPublic | Instance
        /// </summary>
        public BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>
        /// 클래스, 함수, 파라미터 이름을 사용자 입력을 비교 할 때 대 소문자 구별을 정확하게 할 것인지, <br />
        /// 대 소문자 구별을 하지 않을지 정합니다.
        /// </summary>
        public bool IsPerfectNameCheck = false;

        private Option()
        {

        }
    }
}
