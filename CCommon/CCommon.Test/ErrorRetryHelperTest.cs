using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCommon.Common;

namespace CCommon.Test
{
    [TestClass]
    public class ErrorRetryHelperTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string errorRetryConfig = "1,2,1";
            var returnResult = ErrorRetryHelper.Handle(errorRetryConfig, () =>
            {
                return true;
            });

            Assert.IsTrue(returnResult.IsValid);
        }


        [TestMethod]
        public void TestMethod2()
        {
            string errorRetryConfig = "1,2,1";
            var returnResult = ErrorRetryHelper.Handle(errorRetryConfig, () =>
            {
                return ReturnResult.SuccessResult();
            });

            Assert.IsTrue(returnResult.IsValid);
        }


        [TestMethod]
        public void TestMethod3()
        {
            string errorRetryConfig = "1,2,1";
            var returnResult = ErrorRetryHelper.Handle(errorRetryConfig, () =>
            {
                return ReturnResult.FailResult();
            });

            Assert.IsFalse(returnResult.IsValid);
        }
    }
}
