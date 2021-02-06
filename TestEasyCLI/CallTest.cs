using EasyCLI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace TestEasyCLI
{
    [TestClass]
    public class CallTest
    {
        class Foo
        {
            public string bar(int id, string name = "aaa")
            {
                return id + name;
            }

            public string aaa(int ids, string name = "aaa")
            {
                return ids + name;
            }


        }

        [TestMethod]
        public void SimpleTest()
        {
            var c = new EasyCLI.EasyCLI();


            c.AddClass(new Foo());

            for (int i = 0; i < 10000; i++)
            {
                var a = c.Call<string>("foo bar --id 20 --name \\\"aaaa a a a a\\\"");
                if (a != "20hello")
                {
                    Assert.Fail();
                }
            }


        }
    }
}
