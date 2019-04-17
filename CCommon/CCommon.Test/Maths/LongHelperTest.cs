using CCommon.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Test.Maths
{
    /// <summary>
    /// 限流器测试
    /// </summary>
    [TestClass]
    public class LongHelperTest
    {
        [TestMethod]
        public void BitCountTest()
        {

            Assert.IsTrue(LongHelper.BitCount(32L) == 1);
            Assert.IsTrue(LongHelper.BitCount(576460752303423520L) == 2);
            Assert.IsTrue(LongHelper.BitCount(1072) == 3);
            Assert.IsTrue(LongHelper.BitCount(1080) == 4);
            Assert.IsTrue(LongHelper.BitCount(-1024) == 54);

            



            
        }

        /// <summary>
        /// 获取Long高位1的位置(左侧1的位置)
        /// </summary>
        [TestMethod]
        public void NumberOfTrailingZerosTest()
        {
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(1) == 0);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(65) == 0);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(2) == 1);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(4) == 2);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(44) == 2);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(16) == 4);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(48) == 4);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(64) == 6);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros(1L << 63) == 63);
            Assert.IsTrue(LongHelper.NumberOfTrailingZeros((1L << 63) | (1L << 60)) == 60);
        }

        [TestMethod]
        public void UnsignedRightBitMoveTest()
        {
            //右无符号位移操作
            Assert.IsTrue(LongHelper.UnsignedRightBitMove(-4611686018427387904, 32) == 3221225472);
            Assert.IsTrue(LongHelper.UnsignedRightBitMove(100, 3) == 12);
            Assert.IsTrue(LongHelper.UnsignedRightBitMove(-4, 32) == 4294967295);
            Assert.IsTrue(LongHelper.UnsignedRightBitMove(-9223372036854775804, 63) == 1);
        }

        [TestMethod]
        public void SaturateCalculationTest()
        {
            Assert.IsTrue(LongHelper.SaturatedAdd(5, 5) == 10);
            Assert.IsTrue(LongHelper.SaturatedAdd(long.MaxValue, 5) == long.MaxValue);
            Assert.IsTrue(LongHelper.SaturatedAdd(long.MinValue, -5) == long.MinValue);

            Assert.IsTrue(LongHelper.SaturatedSubtract(long.MinValue, 5) == long.MinValue);

            Assert.IsTrue(LongHelper.SaturatedMultiply(5, 5) == 25);
            Assert.IsTrue(LongHelper.SaturatedMultiply(long.MaxValue / 2, 3) == long.MaxValue);
        }
    }
}