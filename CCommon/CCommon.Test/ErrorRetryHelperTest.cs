using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCommon.Common;

namespace CCommon.Test
{
    [TestClass]
    public class ErrorRetryHelperTest
    {
        [TestMethod]
        public void SuccessResultTest1()
        {
            string errorRetryConfig = "1,2,1,6,7";
            var returnResult = ErrorRetryHelper.Handle(errorRetryConfig, () =>
            {
                return true;
            });

            Assert.IsTrue(returnResult.IsValid);
        }


        [TestMethod]
        public void SuccessResultTest2()
        {
            string errorRetryConfig = "1,2,1";
            var returnResult = ErrorRetryHelper.Handle(errorRetryConfig, () =>
            {
                return ReturnResult.SuccessResult();
            });

            Assert.IsTrue(returnResult.IsValid);
        }

        [TestMethod]
        public void SuccessResultTest3()
        {
            string errorRetryConfig = "1,2,1";
            var returnResult = ErrorRetryHelper.Handle(errorRetryConfig, () =>
            {
                return ReturnResult<DateTime>.SuccessResult(new DateTime(2019,1,1));
            });

            Assert.IsTrue(returnResult.IsValid);
            Assert.AreEqual(returnResult.Data, new DateTime(2019, 1, 1));
        }

        [TestMethod]
        public void TestMethod4()
        {
            string errorRetryConfig = "1,2,1";
            var returnResult = ErrorRetryHelper.Handle(errorRetryConfig, () =>
            {
                return ReturnResult.FailResult("aaa");
            });
            Assert.IsFalse(returnResult.IsValid);
        }
    }
}
