using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EasyBlock.Core.Implementations.Settings;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.Utils;
using Sections = EasyBlock.Core.Constants.Sections;
using Keys = EasyBlock.Core.Constants.Keys;
using Defaults = EasyBlock.Core.Constants.Defaults;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests.Settings
{
    [TestFixture]
    public class TestStarterConfigGenerator
    {
        [TearDown]
        public void TearDown()
        {
            var iniPath = (new StarterConfigGenerator().IniFilePath);
            if (File.Exists(iniPath))
                File.Delete(iniPath);
        }

        [TestCase(Sections.SETTINGS, Keys.CACHE_FOLDER, Defaults.CACHE_FOLDER)]
        [TestCase(Sections.SETTINGS, Keys.HOSTS_FILE, Defaults.WINDOWS_HOSTS_FILE_LOCATION)]
        [TestCase(Sections.SETTINGS, Keys.REDIRECT_IP, Defaults.LOCALHOST)]
        [TestCase(Sections.SETTINGS, Keys.REFRESH_INTERVAL_IN_MINUTES, Defaults.ONE_DAY)]
        public void CreateConfigIfNotFound_ShouldCreateConfigWithSettingsSectionWith_(string section, string setting, string expected)
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var executingFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var fileName = Constants.CONFIG_FILE;
            using (var tempFile = new AutoTempFile(executingFolder, fileName, new byte[] { }))
            {
                //---------------Assert Precondition----------------
                if (File.Exists(tempFile.Path))
                    File.Delete(tempFile.Path);
                Assert.AreEqual(tempFile.Path, sut.IniFilePath);

                //---------------Execute Test ----------------------
                sut.CreateConfigIfNotFound();


                //---------------Test Result -----------------------
                var iniFile = new INIFile(tempFile.Path);
                Assert.AreEqual(expected, iniFile[section][setting]);
            }
        }

        [TestCase("https://adaway.org/hosts.txt")]
        [TestCase("http://winhelp2002.mvps.org/hosts.txt")]
        [TestCase("http://hosts-file.net/ad_servers.txt")]
        [TestCase("https://pgl.yoyo.org/adservers/serverlist.php?hostformat=hosts&showintro=0&mimetype=plaintext")]
        public void CreateConfigIfNotFound_ShouldAddDefaultSources(string url)
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            //---------------Assert Precondition----------------
            Assert.IsFalse(File.Exists(sut.IniFilePath));

            //---------------Execute Test ----------------------
            sut.CreateConfigIfNotFound();

            //---------------Test Result -----------------------
            var ini = new INIFile(sut.IniFilePath);
            var allUrls = ini[Sections.SOURCES]
                .Keys.Select(k => ini[Sections.SOURCES][k] == null 
                    ? k 
                    : k + "=" + ini[Sections.SOURCES][k]);
            Assert.IsTrue(allUrls.Contains(url));
        }

        [Test]
        public void CreateConfigIfNotFound_WhenConfigExists_ShouldNotAlterIt()
        {
            //---------------Set up test pack-------------------
            var sut = Create();
            var expected = GetRandomString(10, 20);
            File.WriteAllText(sut.IniFilePath, expected);

            //---------------Assert Precondition----------------
            Assert.IsTrue(File.Exists(sut.IniFilePath));

            //---------------Execute Test ----------------------
            sut.CreateConfigIfNotFound();

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, File.ReadAllText(sut.IniFilePath));
        }



        private StarterConfigGenerator Create()
        {
            return new StarterConfigGenerator();
        }
    }
}
