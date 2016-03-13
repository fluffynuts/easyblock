using Castle.Windsor;
using NSubstitute;
using NUnit.Framework;

namespace EasyBlock.Win32Service.Tests
{
    [TestFixture]
    public class TestEasyBlockService
    {
        [Test]
        public void Construct_GivenContainer_ShouldUseIt()
        {
            //---------------Set up test pack-------------------
            var container = Substitute.For<IWindsorContainer>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = new EasyBlockService(container);

            //---------------Test Result -----------------------
            Assert.AreEqual(container, sut.Container);
        }

        [Test]
        [Ignore("TODO: figure out how to prove this?!")]
        public void DefaultConstructor_ShouldUse_ContainerFrom_WindsorBootstrapper()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }


    }

}
