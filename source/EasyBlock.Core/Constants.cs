﻿namespace EasyBlock.Core
{
    public static class Constants
    {
        public static class Sections
        {
            public const string SETTINGS = "settings";
            public const string SOURCES = "sources";
            public const string BLACKLIST = "blacklist";
            public const string WHITELIST = "whitelist";
        }
        public static class Keys
        {
            public const string REDIRECT_IP = "RedirectIp";
            public const string CACHE_FOLDER = "CacheFolder";
            public const string HOSTS_FILE = "HostsFile";
            public const string REFRESH_INTERVAL_IN_MINUTES = "RefreshIntervalInMinutes";
        }
        public static class Defaults
        {
            public const string CACHE_FOLDER = "cache";
            public const string LOCALHOST = "127.0.0.1";
            public const string WINDOWS_HOSTS_FILE_LOCATION = "%WINDIR%\\system32\\drivers\\etc\\hosts";
            public const string ONE_DAY = "1440"; // one day
        }

        public const string MERGE_MARKER = "# Lines below this have been merged in by EasyBlock and will be removed when the service is stopped.";
        public const string CONFIG_FILE = "EasyBlock.ini";
    }
}