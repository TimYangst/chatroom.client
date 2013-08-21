using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace chatroom.client.test
{

    class DllLoadTester
    {
        [DllImport("D:\\project\\chatroom\\dll\\Project.dll")]
        public static extern int func(int a);
        
    }

    [TestClass]
    public class DllLoadTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(6,  DllLoadTester.func(3));

        }
    }
}
