using EasyCLI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestEasyCLI
{
    [TestClass]
    public class CallTest
    {
        [AlternativeName("bbb")]
        class Foo
        {
            [AlternativeName()]
            string bar(int id, string name = "aaa")
            {
                return id + name;
            }

            string aaa(int id, string name = "aaa")
            {
                return id + name;
            }


        }

        [TestMethod]
        public void SimpleTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new Foo());

            var a = c.Call<string>("bbb --id 20 --name hello");
            if (a != "20hello")
            {
                Assert.Fail();
            }

            a = c.Call<string>("bbb --id 40 --name aaaa");
            if (a != "40aaaa")
            {
                Assert.Fail();
            }

            a = c.Call<string>("bbb --id --name --name aaaa");
            if (a != "--nameaaaa")
            {
                Assert.Fail();
            }

            a = c.Call<string>("bbb --id 20");
            if (a != "20aaa")
            {
                Assert.Fail();
            }


        }
    }
}
