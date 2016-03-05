using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Components.DictionaryAdapter;
using PeanutButter.INIFile;
using static EasyBlock.Core.Constants;

namespace EasyBlock.Core
{
    public class ApplicationConfiguration: IApplicationConfiguration
    {
        public int RefreshIntervalInMinutes { get; private set; }
        public string HostsFile { get; private set; }
        public string CacheFolder { get; private set; }
        public IEnumerable<string> Sources => _sources;
        public IEnumerable<string> Blacklist => _blacklist;
        public IEnumerable<string> Whitelist => _whitelist;
        private readonly List<string> _blacklist = new EditableList<string>();
        private readonly List<string> _whitelist = new EditableList<string>();
        private readonly List<string> _sources = new EditableList<string>();

        public ApplicationConfiguration(IINIFile iniFile)
        {
            if (iniFile == null) throw new ArgumentNullException(nameof(iniFile));
            LoadFrom(iniFile);
        }

        private void LoadFrom(IINIFile iniFile)
        {
            LoadSettingsFrom(iniFile);
            LoadSourcesFrom(iniFile);
            LoadBlacklistFrom(iniFile);
            LoadWhitelistFrom(iniFile);
        }

        private void LoadSourcesFrom(IINIFile iniFile)
        {
            LoadKeysInto(_sources, iniFile, Sections.SOURCES);
        }

        private void LoadWhitelistFrom(IINIFile iniFile)
        {
            LoadKeysInto(_whitelist, iniFile, Sections.WHITELIST);
        }

        private void LoadBlacklistFrom(IINIFile iniFile)
        {
            LoadKeysInto(_blacklist, iniFile, Sections.BLACKLIST);
        }

        private void LoadKeysInto(List<string> target, IINIFile iniFile, string section)
        {
            if (!iniFile.HasSection(section))
                return;
            target.AddRange(iniFile[section].Keys.ToArray());
        }

        private void LoadSettingsFrom(IINIFile iniFile)
        {
            Func<string, string, string> getSetting = (key, defaultValue) =>
                iniFile.GetValue(Sections.SETTINGS, key, defaultValue);
            RefreshIntervalInMinutes = getSetting(
                                            Keys.REFRESH_INTERVAL_IN_MINUTES,
                                            Defaults.ONE_DAY.ToString()
                                        ).AsInteger();
            HostsFile = getSetting(
                Keys.HOSTS_FILE,
                Defaults.WINDOWS_HOSTS_FILE_LOCATION
            );
            CacheFolder = getSetting(Keys.CACHE_FOLDER, DetermineDefaultCacheFolder());
        }

        private string DetermineDefaultCacheFolder()
        {
            var appPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var appFolder = Path.GetDirectoryName(appPath);
            return Path.Combine(appFolder, "cache");
        }
    }

    public static class StringExtensions
    {
        public static int AsInteger(this string value)
        {
            int result;
            int.TryParse(value, out result);
            return result;
        }
    }
}
