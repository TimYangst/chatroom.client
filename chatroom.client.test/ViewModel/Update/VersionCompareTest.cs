using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using chatroom.client.ViewModel.Update;

namespace chatroom.client.test.ViewModel.Update
{

    [TestClass]
    public class VersionCompareTest
    {
        [TestMethod]
        public void  testCompareVersion()
        {
            Assert.IsTrue(CheckUtils.CompareVersion("1.0.12","0.0.3"));
            Assert.IsFalse(CheckUtils.CompareVersion("0.0.2", "0.0.2"));
            Assert.IsTrue(CheckUtils.CompareVersion("0.2", "0.1.2"));
            Assert.IsFalse(CheckUtils.CompareVersion("XXXX", "1.2.3"));
            Assert.IsFalse(CheckUtils.CompareVersion("0.2", "0.2.2"));
            Assert.IsTrue(CheckUtils.CompareVersion("0.2.2", "0.2"));
            Assert.IsTrue(CheckUtils.CompareVersion("3", "2.4.18"));
            Assert.IsFalse(CheckUtils.CompareVersion("1.1.2.123", "1.1.2."));
        }


    }
}
