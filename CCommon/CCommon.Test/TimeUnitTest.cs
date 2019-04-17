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
        public void Test()
        {
            Assert.IsTrue(TimeUnit.Seconds.toMicros(1) == 1 * 1000 * 10000);
            Assert.IsTrue(TimeUnit.Minutes.toMicros(2) == 2 * 1000 * 10000*60);
        }
    }
}
