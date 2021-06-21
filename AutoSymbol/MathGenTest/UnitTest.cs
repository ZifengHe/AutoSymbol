using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathGen;

namespace MathGenTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void OpTree_CreateTemplate()
        {
            OpTree.CreateTemplates(5);
            Assert.IsTrue(OpTree.AllTemplates.Count == 33);            
        }
    }
}
