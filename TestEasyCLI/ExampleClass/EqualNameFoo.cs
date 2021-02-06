using EasyCLI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestEasyCLI.ExampleClass
{
    class EqualNameFoo
    {
        [AlternativeName("h")]
        public string bar(int id, string name)
        {
            return id + name + "bar";
        }

        [AlternativeName("h")]
        public string aaa(int id, string name = "aaa")
        {
            return id + name + "ccc";
        }
    }
}
