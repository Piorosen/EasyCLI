using System;
using System.Collections.Generic;
using System.Text;

namespace TestEasyCLI.ExampleClass
{
    class Foo
    {
        public string bar(int id, string name)
        {
            return id + name + "bar";
        }

        public string aaa(int ids, string name = "aaa")
        {
            return ids + name + "ccc";
        }
    }
}
