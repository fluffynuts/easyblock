using System;
using System.IO;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.TestUtils.Generic;
using static PeanutButter.RandomGenerators.RandomValueGen;
using static EasyBlock.Core.Constants;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestAppSettings
    {
        [Test]
        public void Type_ShouldImplement_IAppSettings()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(ApplicationConfiguration);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<IApplicationConfiguration>();

            //---------------Test Result -----------------------
        }

        [TestCase("iniFile", typeof(IINIFile))]
        public void Construct_ShouldRequire_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<ApplicationConfiguration>(parameterName, parameterType);

            //---------------Test Result -----------------------
        }

        [Test]
        public void Construct_ShouldLoadRefreshIntervalInMinutes_FromReader()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var expected = GetRandomInt();
            iniFile.SetValue(Constants.Sections.SETTINGS, Constants.Keys.REFRESH_INTERVAL_IN_MINUTES, expected.ToString());

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(iniFile);
            var result = sut.RefreshIntervalInMinutes;

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Construct_WhenNoRefreshIntervalInIniFile_ShouldUseOneDay()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(iniFile);
            var result = sut.RefreshIntervalInMinutes;

            //---------------Test Result -----------------------
            Assert.AreEqual(1440, result);
        }

        [Test]
        public void Construct_WhenNoHostFileSetting_ShouldUseWindowsHostFileLocation()
        {
            //---------------Set up test pack-------------------
            var expected = "%WINDIR%\\system32\\drivers\\etc\\hosts";
            var iniFile = new INIFile();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(iniFile);
            var result = sut.HostsFile;

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Construct_WhenHaveCacheFolderSetting_ShouldSetCacheFolderFromThatSetting()
        {
            //---------------Set up test pack-------------------
            var ini = new INIFile();
            var expected = GetRandomWindowsPath();
            ini.SetValue(Sections.SETTINGS, Keys.CACHE_FOLDER, expected);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(ini);
            var result = sut.CacheFolder;

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Construct_WhenHaveNoCacheFolderSetting_ShouldUseAppFolderPlusCache()
        {
            //---------------Set up test pack-------------------
            var ini = new INIFile();
            var appPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var appFolder = Path.GetDirectoryName(appPath);
            var expected = Path.Combine(appFolder, "cache");

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(ini);
            var result = sut.CacheFolder;

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void Construct_WhenHaveHostFileSetting_ShouldUseThatSetting()
        {
            //---------------Set up test pack-------------------
            var expected = GetRandomString();
            var iniFile = new INIFile();
            iniFile.SetValue(Sections.SETTINGS, Keys.HOSTS_FILE, expected);
            var sut = Create(iniFile);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = sut.HostsFile;

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Construct_WhenNoBlacklistsSection_ShouldhaveEmptyBlackLists()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var sut = Create(iniFile);


            //---------------Assert Precondition----------------
            Assert.IsFalse(iniFile.HasSection(Sections.BLACKLIST));

            //---------------Execute Test ----------------------
            var result = sut.Blacklist;

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Construct_WhenHaveBlacklistsSection_ShouldLoadIt()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var host1 = GetRandomHostname();
            var host2 = GetRandomHostname();
            iniFile[Sections.BLACKLIST][host1] = null;
            iniFile[Sections.BLACKLIST][host2] = null;
            var sut = Create(iniFile);


            //---------------Assert Precondition----------------
            Assert.IsTrue(iniFile.HasSection(Sections.BLACKLIST));

            //---------------Execute Test ----------------------
            var result = sut.Blacklist;

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.Contains(result, host1);
            CollectionAssert.Contains(result, host2);
        }


        [Test]
        public void Construct_WhenNoWhitelistsSection_ShouldhaveEmptyWhiteLists()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var sut = Create(iniFile);


            //---------------Assert Precondition----------------
            Assert.IsFalse(iniFile.HasSection(Sections.BLACKLIST));

            //---------------Execute Test ----------------------
            var result = sut.Whitelist;

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Construct_WhenHaveWhitelistsSection_ShouldLoadIt()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var host1 = GetRandomHostname();
            var host2 = GetRandomHostname();
            iniFile[Sections.WHITELIST][host1] = null;
            iniFile[Sections.WHITELIST][host2] = null;
            var sut = Create(iniFile);


            //---------------Assert Precondition----------------
            Assert.IsTrue(iniFile.HasSection(Sections.WHITELIST));

            //---------------Execute Test ----------------------
            var result = sut.Whitelist;

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.Contains(result, host1);
            CollectionAssert.Contains(result, host2);
        }

        private IApplicationConfiguration Create(IINIFile iniFile)
        {
            return new ApplicationConfiguration(iniFile);
        }
    }

}
