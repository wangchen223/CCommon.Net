using CCommon.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCommon.Test
{
    /// <summary>
    /// 时间单位处理
    /// </summary>
    [TestClass]
    public class TimeUnitTest
    {

        [TestMethod]
        public void ToMicrosTest()
        {
            Assert.IsTrue(TimeUnit.Seconds.toMicros(1) == 1 * 1000 * 10000);
            Assert.IsTrue(TimeUnit.Minutes.toMicros(2) == 2 * 1000 * 10000*60);
        }

        [TestMethod]
        public void ToMicrosecondsTest()
        {
            Assert.IsTrue(TimeUnit.Seconds.toMicroseconds(1) == 1000);
            Assert.IsTrue(TimeUnit.Minutes.toMicroseconds(1) == 1000 * 60);
        }


        [TestMethod]
        public void ToSecondsTest()
        {
            Assert.IsTrue(TimeUnit.Seconds.toSeconds(1) == 1);
            Assert.IsTrue(TimeUnit.Minutes.toSeconds(1) == 1 * 60);
        }
    }
}
