using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCommon.Common;
namespace CCommon.Test
{
    /// <summary>
    /// Json助手测试
    /// </summary>
    [TestClass]
    public class JsonHelperTest
    {
        [TestMethod]
        public void JsonTest()
        {
            Assert.AreEqual(@"{""UserName"":""测试""}", new JsonTestDTO { UserName = "测试" }.ToJson());

            Assert.AreEqual(@"5", 5.ToJson());

            Assert.AreEqual(@"""5""", "5".ToJson());
        }
    }

    public class JsonTestDTO
    {
        public string UserName { get; set; }
    }
}
