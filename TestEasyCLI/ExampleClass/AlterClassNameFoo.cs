using EasyCLI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestEasyCLI.ExampleClass
{
    [AlternativeName("AlterFoo")]
    class AlterClassNameFoo
    {
        public string bar(int id, string name)
        {
            return id + name + "alterfoo";
        }

        public string aaa(int ids, string name = "aaa")
        {
            return ids + name + "alterfoo";
        }
    }
}
