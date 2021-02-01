using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestEasyCLI
{
    [TestClass]
    public class CallTest
    {
        class Foo
        {
            int bar(int id, string name)
            {
                return 1000;
            }
        }
        [TestMethod]
        public void SimpleTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new Foo());
            c.Call("foo bar --id 20 --name hello");
        }
    }
}
