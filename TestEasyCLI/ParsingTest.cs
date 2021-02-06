using EasyCLI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestEasyCLI
{
    [TestClass]
    public class ParsingTest
    {
        [TestMethod]
        public void SimpleTest()
        {
            var r = "a b c d e".Arguments();
            if (r.Length != 5)
            {
                Assert.Fail("개수가 5개가 아님.");
            }


            if (r[0] != "a" ||
                r[1] != "b" ||
                r[2] != "c" ||
                r[3] != "d" ||
                r[4] != "e")
            {
                Assert.Fail("실패함.");
            }
        }

        [TestMethod]
        public void NothingTest()
        {
            var r = "           ".Arguments();
            if (r.Length != 0)
            {
                Assert.Fail("개수가 0개가 아님.");
            }
        }

        [TestMethod]
        public void BlankTest()
        {
            var r = "   a    b    c   d e       ".Arguments();
            if (r.Length != 5)
            {
                Assert.Fail("개수가 5개가 아님.");
            }


            if (r[0] != "a" ||
                r[1] != "b" ||
                r[2] != "c" ||
                r[3] != "d" ||
                r[4] != "e")
            {
                Assert.Fail("실패함.");
            }
        }


        [TestMethod]
        public void HeavyTest()
        {
            var r = "aaaa asf \"ha aaaa aa\" ddd\\t \'aaa d \" \" \\\'\' aa".Arguments();
            if (r.Length != 6)
            {
                Assert.Fail("개수가 6개가 아님.");
            }
            if (r[0] != "aaaa" ||
                r[1] != "asf" ||
                r[2] != "ha aaaa aa" ||
                r[3] != "ddd\t" ||
                r[4] != "aaa d \" \" \'" ||
                r[5] != "aa")
            {
                Assert.Fail("데이터를 잘못 추출 함.");
            }
        }
    }
}
