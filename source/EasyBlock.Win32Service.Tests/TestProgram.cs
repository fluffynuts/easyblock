using System.Linq;
using NUnit.Framework;
using PeanutButter.ServiceShell;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Win32Service.Tests
{
    [TestFixture]
    public class TestProgram
    {
        [Test]
        public void Main_ShouldRunEasyBlockServiceMainWithProvidedArguments()
        {
            //---------------Set up test pack-------------------
            Shell.StartTesting();
            var args = GetRandomCollection<string>(2,4).ToArray();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Program.Main(args);

            //---------------Test Result -----------------------
            Shell.ShouldHaveRunMainFor<EasyBlockService>(args);
        }

    }
}
