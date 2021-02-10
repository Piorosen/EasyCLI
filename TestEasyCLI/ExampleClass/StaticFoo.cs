using System;
using System.Collections.Generic;
using System.Text;

namespace TestEasyCLI.ExampleClass
{
    class StaticFoo
    {
        public static string bar(int id, string name)
        {
            return id + name + "bar";
        }

        public static string aaa(int ids, string name = "aaa")
        {
            return ids + name + "ccc";
        }
    }
}
