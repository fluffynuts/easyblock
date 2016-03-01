using System;
using PeanutButter.INIFile;
using PeanutButter.RandomGenerators;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace EasyBlock.Core.Tests
{
    public class IniBuilder
    {
        public static IniBuilder Create()
        {
            return new IniBuilder();
        }

        private string[] _sources;
        private string[] _blacklist;
        private string[] _whitelist;
        private int _interval;
        private string _hostFile;

        public IniBuilder()
        {
            WithSources(GetRandomHttpUrl(), GetRandomHttpUrl())
                .WithInterval(60)
                .WithHostsFile("%SYSTEMROOT%\\system32\\drivers\\etc\\hosts")
                .WithBlacklist()
                .WithWhitelist();

        }

        public IniBuilder WithHostsFile(string path)
        {
            _hostFile = path;
            return this;
        }

        public IniBuilder WithInterval(int interval)
        {
            _interval = interval;
            return this;
        }

        public IniBuilder WithBlacklist(params string[] hosts)
        {
            _blacklist = hosts;
            return this;
        }

        public IniBuilder WithWhitelist(params string[] hosts)
        {
            _whitelist = hosts;
            return this;
        }

        public IniBuilder WithSources(params string[] urls)
        {
            _sources = urls;
            return this;
        }

        public string[] BuildLines()
        {
            return new[] { "[settings]" }
                .And(Config("IntervalInMinutes", _interval.ToString()))
                .And(Config("HostsFile", _hostFile))
                .And("[sources]")
                .And(_sources)
                .And("[blacklist]")
                .And(_blacklist)
                .And("[whitelist]")
                .And(_whitelist);
        }

        public IINIFile Build()
        {
            var iniFile = new INIFile();
            iniFile.Parse(string.Join(Environment.NewLine, BuildLines()));
            return iniFile;
        }

        private string Config(string key, string value)
        {
            return $"{key}={value}";
        }
    }
}