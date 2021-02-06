using EasyCLI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestEasyCLI.ExampleClass
{
    [AlternativeName("haaa")]
    class AlterMethodParamClassFoo
    {
        [AlternativeName("ccc")]
        public string bar([AlternativeName("idha!")] int id, 
                          [AlternativeName("nname")] string name)
        {
            return id + name + "bar";
        }

        [AlternativeName("ccc")]
        public string aaa(int ids, string name = "aaa")
        {
            return ids + name + "ccc";
        }

        [AlternativeName()]
        public string fff(int ids, string name = "aaa")
        {
            return ids + name + "fff";
        }
    }
}
