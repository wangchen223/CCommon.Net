using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCommon.Common.Maths;

namespace CCommon.Test.Maths
{
    [TestClass]
    public class IntHelperTest
    {
        [TestMethod]
        public void UnsignedRightBitMoveTest()
        {
            //右无符号位移操作
            Assert.IsTrue(IntHelper.UnsignedRightBitMove(100, 3) == 12);
        }
    }
}
