using EasyCLI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using TestEasyCLI.ExampleClass;

namespace TestEasyCLI
{
    

    [TestClass]
    public class CallTest
    {
        [TestMethod]
        public void StaticMethodTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new Foo());
            c.AddClass(new StaticFoo());

            var e = c.Call<string>("staticfoo bar --id 20 --name ha");

        }

        protected void ExceptionMessage(Action action, string message)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Assert.AreEqual(message, e.Message);
                return;
            }
            Assert.Fail("예외가 발생하지 않았습니다.");
            return;
        }


        [TestMethod]
        public void MethodAndParamNameTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new AlterMethodParamClassFoo());
            c.AddClass(new Foo());

            // ALTER FOO CLASS TEST
            Assert.AreEqual(c.Call<string>("haaa --ids 20"), "20aaafff");
            Assert.AreEqual(c.Call<string>("haaa --ids 40 --name hhhh"), "40hhhhfff");
            Assert.AreEqual(c.Call<string>("haaa --ids 20 --name"), "20aaafff");

            Assert.AreEqual(c.Call<string>("haaa ccc --idha! 20 --nname 2312"), "202312bar");
            Assert.AreEqual(c.Call<string>("haaa ccc --idha! 20 --nname 5Aas"), "205Aasbar");
            Assert.AreEqual(c.Call<string>("haaa ccc --idha! 20 --nname hhhhh"), "20hhhhhbar");

            Assert.AreEqual(c.Call<string>("haaa ccc --ids 30 --name \"aaba\""), "30aabaccc");
            Assert.AreEqual(c.Call<string>("haaa ccc --ids 30 --name \"ff f f\""), "30ff f fccc");
            Assert.AreEqual(c.Call<string>("haaa ccc --ids 30 --name"), "30aaaccc");
        }

        [TestMethod]
        public void ErrorCatchTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new EqualNameFoo());

            ExceptionMessage(() =>
            {
                c.Call<string>("EqualNameFoo h --id --name");
            }, "일치하는 클래스가 1개 이지만 일치하는 메소드가 2개 이상이거나 없습니다.");

            c.AddClass(new EqualNameFoo());

            ExceptionMessage(() =>
            {
                c.Call<string>("EqualNameFoo h --id --name");
            }, "일치하는 클래스의 종류가 2개 이상이거나 없습니다.");
        }

        [TestMethod]
        public void ChangePositionParamTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new Foo());

            Assert.AreEqual(c.Call<string>("Foo AAA --name a --ids 222 "), "222accc");
            Assert.AreEqual(c.Call<string>("foo aaa --name --ids 100 "), "100aaaccc");
            Assert.AreEqual(c.Call<string>("foo aaa --name null --ids 100 "), "100nullccc");
        }

        [TestMethod]
        public void AlternativeNameClassTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new AlterClassNameFoo());
            c.AddClass(new Foo());

            // ALTER FOO CLASS TEST
            Assert.AreEqual(c.Call<string>("alterFoo baR --id 20 --name \"aaaa \\\"a a a a\""), "20aaaa \"a a a aalterfoo");
            Assert.AreEqual(c.Call<string>("AlterFoo bAr --id 222 --name a"), "222aalterfoo");
            Assert.AreEqual(c.Call<string>("ALterFoo aaA --iDs 20"), "20aaaalterfoo");
            Assert.AreEqual(c.Call<string>("AlTerFoo AAA --idS 222 --name a"), "222aalterfoo");
            Assert.AreEqual(c.Call<string>("AltErFoo aAa --ids 100 --naMe"), "100aaaalterfoo");
            Assert.AreEqual(c.Call<string>("AlteRFoo aaa --Ids 100 --nAme null"), "100nullalterfoo");

            // FOO CLASS TEST
            Assert.AreEqual(c.Call<string>("foo bar --id 20 --name \"aaaa \\\"a a a a\""), "20aaaa \"a a a abar");
            Assert.AreEqual(c.Call<string>("Foo bar --id 222 --name a"), "222abar");
            Assert.AreEqual(c.Call<string>("foo aaa --ids 20"), "20aaaccc");
            Assert.AreEqual(c.Call<string>("Foo AAA --ids 222 --name a"), "222accc");
            Assert.AreEqual(c.Call<string>("foo aaa --ids 100 --name"), "100aaaccc");
            Assert.AreEqual(c.Call<string>("foo aaa --ids 100 --name null"), "100nullccc");
        }

        [TestMethod]
        public void FooBarCallTest()
        {
            var c = new EasyCLI.EasyCLI();
            c.AddClass(new Foo());
            Assert.AreEqual(c.Call<string>("foo bar --id 20 --name \"aaaa \\\"a a a a\""), "20aaaa \"a a a abar");
            Assert.AreEqual(c.Call<string>("Foo bar --id 222 --name a"), "222abar");
        }

        [TestMethod]
        public void FooAaaCallTest()
        {
            var c = new EasyCLI.EasyCLI();

            c.AddClass(new Foo());

            Assert.AreEqual(c.Call<string>("foo aaa --ids 20"), "20aaaccc");
            Assert.AreEqual(c.Call<string>("Foo AAA --ids 222 --name a"), "222accc");
            Assert.AreEqual(c.Call<string>("foo aaa --ids 100 --name"), "100aaaccc");
            Assert.AreEqual(c.Call<string>("foo aaa --ids 100 --name null"), "100nullccc");
        }
    }
}
