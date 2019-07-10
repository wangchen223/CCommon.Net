using CCommon.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
namespace CCommon.Test
{
    public enum EColor
    {
        [System.ComponentModel.Description("红色")]
        Red=1,
        [System.ComponentModel.Description("黄色")]
        Yellow = 2
    }
    [TestClass]
    public class EnumHelperTest
    {

        [TestMethod]
        public void Test()
        {
            Assert.AreEqual("红色", EnumHelper.GetDescription(EColor.Red));
            Assert.AreEqual("黄色", EnumHelper.GetDescription(EColor.Yellow));
        }
    }
}
