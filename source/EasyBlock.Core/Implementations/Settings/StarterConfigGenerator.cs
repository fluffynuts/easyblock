using System.Collections.Generic;
using System.IO;
using PeanutButter.INIFile;
using PeanutButter.Utils;

namespace EasyBlock.Core.Implementations.IO.Settings
{
    public class StarterConfigGenerator
    {
        public string IniFilePath => Path.Combine(ExecutingAssemblyPathFinder.GetExecutingAssemblyFolder(), 
                                                    Constants.CONFIG_FILE);
        private static readonly string[] DefaultSources  = new[]
        {
            "https://adaway.org/hosts.txt",
            "http://winhelp2002.mvps.org/hosts.txt",
            "http://hosts-file.net/ad_servers.txt",
            "https://pgl.yoyo.org/adservers/serverlist.php?hostformat=hosts&showintro=0&mimetype=plaintext"
        };

        public void CreateConfigIfNotFound()
        {
            if (File.Exists(IniFilePath))
                return;
            var iniFile = new INIFile(IniFilePath);
            SetDefaultSettingsOn(iniFile);
            SetDefaultSourcesOn(iniFile);
            iniFile.Persist();
        }

        private void SetDefaultSourcesOn(INIFile iniFile)
        {
            DefaultSources.ForEach(s => iniFile[Constants.Sections.SOURCES][s] = null);
        }

        private static void SetDefaultSettingsOn(INIFile iniFile)
        {
            new Dictionary<string, string>()
            {
                { Constants.Keys.CACHE_FOLDER, Constants.Defaults.CACHE_FOLDER },
                { Constants.Keys.HOSTS_FILE, Constants.Defaults.WINDOWS_HOSTS_FILE_LOCATION },
                { Constants.Keys.REDIRECT_IP, Constants.Defaults.LOCALHOST },
                { Constants.Keys.REFRESH_INTERVAL_IN_MINUTES, Constants.Defaults.ONE_DAY }
            }.ForEach(kvp => iniFile[Constants.Sections.SETTINGS][kvp.Key] = kvp.Value);
        }

    }
}
