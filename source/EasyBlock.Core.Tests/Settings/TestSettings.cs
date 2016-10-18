using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EasyBlock.Core.Implementations.IO.Settings;
using EasyBlock.Core.Interfaces.IO.Settings;
using NUnit.Framework;
using PeanutButter.INIFile;
using PeanutButter.TestUtils.Generic;
using static PeanutButter.RandomGenerators.RandomValueGen;
using static EasyBlock.Core.Constants;

namespace EasyBlock.Core.Tests
{
    [TestFixture]
    public class TestSettings
    {
        [Test]
        public void Type_ShouldImplement_IAppSettings()
        {
            //---------------Set up test pack-------------------
            var sut = typeof(Settings);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            sut.ShouldImplement<ISettings>();

            //---------------Test Result -----------------------
        }

        [TestCase("iniFile", typeof(IINIFile))]
        public void Construct_ShouldRequire_(string parameterName, Type parameterType)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            ConstructorTestUtils.ShouldExpectNonNullParameterFor<Settings>(parameterName, parameterType);

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
            var expected = Environment.ExpandEnvironmentVariables("%WINDIR%\\system32\\drivers\\etc\\hosts");
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

        [Test]
        public void Construct_ShouldLoadSourcesFromIniFile()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var source1 = GetRandomHttpUrl();
            var source2 = GetRandomHttpUrl();
            iniFile[Sections.SOURCES][source1] = null;
            iniFile[Sections.SOURCES][source2] = null;
            var sut = Create(iniFile);

            //---------------Assert Precondition----------------
            Assert.IsTrue(iniFile.HasSection(Sections.SOURCES));

            //---------------Execute Test ----------------------
            var result = sut.Sources;

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AreEqual(new[] { source1, source2 }, result);
        }

        [Test]
        public void Construct_WhenSourceHasKeyAndValue_ShouldUseCombinedResult()
        {
            // happens as a result of the (mis)use of the ini format:
            // http://somehost.somedomain/route?var1=var2
            //  will be interpreted by INIFile into:
            //  key: http://somehost.somedomain/route?var1
            //  value: var2
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var start = GetRandomHttpUrl() + "?somevar";
            var value = "somevalue";
            var expected = start + "=" + value;
            iniFile[Sections.SOURCES][start] = value;
            var sut = Create(iniFile);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = sut.Sources.Single();

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void Construct_WhenHaveNoSourcesSection_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            var sut = Create(iniFile);

            //---------------Assert Precondition----------------
            Assert.IsFalse(iniFile.HasSection(Sections.SOURCES));

            //---------------Execute Test ----------------------
            var result = sut.Sources;

            //---------------Test Result -----------------------
            CollectionAssert.IsEmpty(result);
        }


        [Test]
        public void Construct_WhenHaveNoRedirectIp_ShouldSetTo_127_0_0_1()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();

            //---------------Assert Precondition----------------
            Assert.IsFalse(iniFile.HasSection(Sections.SETTINGS));

            //---------------Execute Test ----------------------
            var sut = Create(iniFile);
            var result = sut.RedirectIp;

            //---------------Test Result -----------------------
            Assert.AreEqual(Defaults.LOCALHOST, result);
        }

        [Test]
        public void Construct_WhenHaveInvalidRedirectIp_ShouldSetTo_127_0_0_1()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            iniFile["settings"]["RedirectIp"] = GetRandomString();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(iniFile);
            var result = sut.RedirectIp;

            //---------------Test Result -----------------------
            Assert.AreEqual(Defaults.LOCALHOST, result);
        }

        [Test]
        public void Construct_WhenHaveInvalidRedirectIp2_ShouldSetTo_127_0_0_1()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            iniFile["settings"]["RedirectIp"] = "127.0.0.256";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(iniFile);
            var result = sut.RedirectIp;

            //---------------Test Result -----------------------
            Assert.AreEqual(Defaults.LOCALHOST, result);
        }

        [Test]
        public void Construct_WhenHaveInvalidRedirectIp3_ShouldSetTo_127_0_0_1()
        {
            //---------------Set up test pack-------------------
            var iniFile = new INIFile();
            iniFile["settings"]["RedirectIp"] = "127.0.0.256a";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var sut = Create(iniFile);
            var result = sut.RedirectIp;

            //---------------Test Result -----------------------
            Assert.AreEqual(Defaults.LOCALHOST, result);
        }



        private ISettings Create(IINIFile iniFile)
        {
            return new Settings(iniFile);
        }
    }
}
