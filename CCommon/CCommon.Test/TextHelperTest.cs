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
    public class TextHelperTest
    {

        [TestMethod]
        public void SubstringTest()
        {
            Assert.AreEqual("CCommon", TextHelper.Substring("CCommon.Test", 7));
            Assert.AreEqual("CCom...", TextHelper.Substring("CCommon.Test", 7,"..."));
            Assert.AreEqual("基础...", TextHelper.Substring("基础通用库测试", 7, "..."));
        }
    }
}
