using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCommon.Common.RateLimiter;
using System.Threading.Tasks;

namespace CCommon.Test
{
    /// <summary>
    /// 限流器测试
    /// </summary>
    [TestClass]
    public class RateLimiterTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            DateTime beginDate = DateTime.Now;
            RateLimiter rl = RateLimiter.create(1);
            rl.acquire();
            rl.acquire();
            rl.acquire();
            rl.acquire();
            TimeSpan time = DateTime.Now - beginDate;
            Assert.IsTrue(time.TotalSeconds >= 3);

        }

        [TestMethod]
        public void TestMethod2()
        {
            DateTime beginDate = DateTime.Now;
            RateLimiter rl = RateLimiter.create(1);
            Assert.IsTrue(rl.tryAcquire());
            Assert.IsFalse(rl.tryAcquire());
        }

        [TestMethod]
        public void TestMethod3()
        {
            DateTime beginDate = DateTime.Now;
            RateLimiter rl = RateLimiter.create(1);
            Assert.IsTrue(rl.tryAcquire());
            Assert.IsTrue(rl.tryAcquire(1,Common.TimeUnit.Seconds));
        }
    }
}
