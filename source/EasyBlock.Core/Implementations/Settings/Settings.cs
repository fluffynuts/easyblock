using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using EasyBlock.Core.Extensions;
using EasyBlock.Core.Interfaces.IO.Settings;
using PeanutButter.INIFile;

namespace EasyBlock.Core.Implementations.IO.Settings
{
    public class Settings: ISettings
    {
        public int RefreshIntervalInMinutes { get; private set; }
        public string HostsFile { get; private set; }
        public string CacheFolder { get; private set; }
        public IEnumerable<string> Sources => _sources;
        public IEnumerable<string> Blacklist => _blacklist;
        public IEnumerable<string> Whitelist => _whitelist;
        public string RedirectIp { get; private set; }
        private readonly List<string> _blacklist = new EditableList<string>();
        private readonly List<string> _whitelist = new EditableList<string>();
        private readonly List<string> _sources = new EditableList<string>();

        public Settings(IINIFile iniFile)
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
            LoadRawLinesInto(_sources, iniFile, Constants.Sections.SOURCES);
        }

        private void LoadWhitelistFrom(IINIFile iniFile)
        {
            LoadRawLinesInto(_whitelist, iniFile, Constants.Sections.WHITELIST);
        }

        private void LoadBlacklistFrom(IINIFile iniFile)
        {
            LoadRawLinesInto(_blacklist, iniFile, Constants.Sections.BLACKLIST);
        }

        private void LoadRawLinesInto(List<string> target, IINIFile iniFile, string section)
        {
            if (!iniFile.HasSection(section))
                return;
            var lines = iniFile[section].Keys.Select(k => GetFullLine(k, iniFile[section]));
            target.AddRange(lines);
        }

        private string GetFullLine(string key, Dictionary<string, string> section)
        {
            var value = section[key];
            return value == null ? key : key + "=" + value;
        }

        private void LoadSettingsFrom(IINIFile iniFile)
        {
            Func<string, string, string> getSetting = (key, defaultValue) =>
                iniFile.GetValue(Constants.Sections.SETTINGS, key, defaultValue);
            RefreshIntervalInMinutes = getSetting(
                                            Constants.Keys.REFRESH_INTERVAL_IN_MINUTES,
                                            Constants.Defaults.ONE_DAY.ToString()
                                        ).AsInteger();
            HostsFile = Environment.ExpandEnvironmentVariables(getSetting(
                Constants.Keys.HOSTS_FILE,
                Constants.Defaults.WINDOWS_HOSTS_FILE_LOCATION
            ));
            CacheFolder = getSetting(Constants.Keys.CACHE_FOLDER, DetermineDefaultCacheFolder());
            var redirectIp = getSetting(Constants.Keys.REDIRECT_IP, Constants.Defaults.LOCALHOST);
            RedirectIp = IsValidIp(redirectIp) ? redirectIp : Constants.Defaults.LOCALHOST;
        }

        private bool IsValidIp(string redirectIp)
        {
            var parts = (redirectIp ?? "").Split('.');
            return parts.Length == 4 &&
                    parts.All(IsOctetValue);
        }

        private bool IsOctetValue(string stringValue)
        {
            int intValue;
            if (int.TryParse(stringValue, out intValue))
                return intValue > -1 && intValue < 256;
            return false;
        }

        private string DetermineDefaultCacheFolder()
        {
            var appFolder = ExecutingAssemblyPathFinder.GetExecutingAssemblyFolder();
            return Path.Combine(appFolder, "cache");
        }
    }
}
