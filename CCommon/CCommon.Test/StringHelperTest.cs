using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCommon.Common;

namespace CCommon.Test
{
    /// <summary>
    /// TextHelperTest 的摘要说明
    /// </summary>
    [TestClass]
    public class StringHelperTest
    {

        [TestMethod]
        public void SubstringTest()
        {
            Assert.AreEqual("CCommon", StringHelper.Substring("CCommon.Test", 7));
            Assert.AreEqual("CCom...", StringHelper.Substring("CCommon.Test", 7,"..."));
            Assert.AreEqual("基础...", StringHelper.Substring("基础通用库测试", 7, "..."));
        }

        [TestMethod]
        public void CommonPrefixTest()
        {
            Assert.AreEqual("abcd", StringHelper.CommonPrefix("abcdabc", "abcddeeffabc"));
            Assert.AreEqual("aaa", StringHelper.CommonPrefix("aaaaa", "aaa"));
            Assert.AreEqual("", StringHelper.CommonPrefix("caaa", "aaacaa"));
        }

        [TestMethod]
        public void CommonSuffixTest()
        {
            Assert.AreEqual("abc", StringHelper.CommonSuffix("aacdabc", "aacddeeffabc"));
            Assert.AreEqual("add", StringHelper.CommonSuffix("aaaaadd", "add"));
            Assert.AreEqual("ad", StringHelper.CommonSuffix("aaad", "ccad"));
        }
    }
}
