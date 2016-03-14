using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.Utils;
using Sections = EasyBlock.Core.Constants.Sections;
using Keys = EasyBlock.Core.Constants.Keys;
using Defaults = EasyBlock.Core.Constants.Defaults;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestStarterConfigGenerator
    {
        [Ignore("WIP")]
        [TestCase(Sections.SETTINGS, Keys.CACHE_FOLDER, Defaults.CACHE_FOLDER)]
        public void CreateConfig_ShouldCreateConfigWithSettingsSectionWith_(string section, string setting, string expected)
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var executingFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var fileName = Constants.CONFIG_FILE;
            var expectedPath = Path.Combine(executingFolder, fileName);
            using (var tempFile = new AutoTempFile(executingFolder, fileName))
            {
                //---------------Assert Precondition----------------
                if (File.Exists(tempFile.Path))
                    File.Delete(tempFile.Path);

                //---------------Execute Test ----------------------
                sut.CreateConfig();


                //---------------Test Result -----------------------
                var iniFile = new INIFile(tempFile.Path);
                Assert.AreEqual(expected, iniFile[section][setting]);
            }
        }

        private StarterConfigGenerator Create()
        {
            return new StarterConfigGenerator();
        }
    }
}
