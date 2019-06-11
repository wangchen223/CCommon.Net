using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCommon.Common.Hash;
namespace CCommon.Test
{
    [TestClass]
    public class MurmurHash3Test
    {
        [TestMethod]
        public void GetHashCode_x64_128Test()
        {
            var hashValue=MurmurHash3.GetHashCode_x64_128("test");
            Assert.AreEqual(hashValue[0], 13464208471605326795u);
            Assert.AreEqual(hashValue[1], 8223150439165006455u);

            hashValue = MurmurHash3.GetHashCode_x64_128("http://www.baidu.com");
            Assert.AreEqual(hashValue[0], 13713873800224469502u);
            Assert.AreEqual(hashValue[1], 2516122795859617196u);

            hashValue = MurmurHash3.GetHashCode_x64_128("测试");
            Assert.AreEqual(hashValue[0], 537293914757730622u);
            Assert.AreEqual(hashValue[1], 11059359174272040356u);
        }


        [TestMethod]
        public void GetHashCode_x86_32Test()
        {
            var hashValue = MurmurHash3.GetHashCode_x86_32("test");
            Assert.AreEqual(hashValue, 3959873882u);

            hashValue = MurmurHash3.GetHashCode_x86_32("http://www.baidu.com");
            Assert.AreEqual(hashValue, 859194221u);

            hashValue = MurmurHash3.GetHashCode_x86_32("测试");
            Assert.AreEqual(hashValue, 2084747940u);
        }

        [TestMethod]
        public void GetHashCode_x86_128Test()
        {
            var hashValue = MurmurHash3.GetHashCode_x86_128("test");
            Assert.AreEqual(hashValue[0], 1501425561u);
            Assert.AreEqual(hashValue[1], 179015912u);
            Assert.AreEqual(hashValue[2], 179015912u);
            Assert.AreEqual(hashValue[3], 179015912u);

            hashValue = MurmurHash3.GetHashCode_x86_128("http://www.baidu.com");
            Assert.AreEqual(hashValue[0], 4027659537u);
            Assert.AreEqual(hashValue[1], 2709780129u);
            Assert.AreEqual(hashValue[2], 3328310823u);
            Assert.AreEqual(hashValue[3], 888702207u);

            hashValue = MurmurHash3.GetHashCode_x86_128("测试");
            Assert.AreEqual(hashValue[0], 2644943207u);
            Assert.AreEqual(hashValue[1], 752160330u);
            Assert.AreEqual(hashValue[2], 2356085005u);
            Assert.AreEqual(hashValue[3], 2356085005u);
        }



    }
}
