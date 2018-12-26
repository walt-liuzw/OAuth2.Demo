using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OAuth2.Demo.Common.Proxy;

namespace OAuth2.Demo.WebTests
{
    [TestClass]
    public class CommonTest
    {
        [TestMethod]
        public void TestAccountProxy()
        {
            string name = "liuzhiwei@bs.com";
            var a = AccountProxy.GetAssociatedUsersByLoginName(name);
        }
    }
}
