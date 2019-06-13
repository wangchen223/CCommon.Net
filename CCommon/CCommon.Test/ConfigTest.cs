using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCommon.Common.Config;

namespace CCommon.Test
{

    [TestClass]
    public class ConfigTest
    {

        [TestMethod]
        public void TestMethod1()
        {
            var config = new ConfigurationBuild()
                .ExceptionTip(true)
                .AddJsonFile("ConfigInfo/appsettings.json")
                .AddXmlFile("ConfigInfo/AppSettingsConfig.xml").Build();
            Assert.AreEqual(config.GetValue<string>("SystemInfo"), "基础库");
            Assert.AreEqual(config.GetValue<string>("ProjectName"), "测试");
            Assert.AreEqual(config.GetValue<Dictionary<string,string>>("ConnectionStrings")["EasySystemConnectionString"], "EasySystemConnection");
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
