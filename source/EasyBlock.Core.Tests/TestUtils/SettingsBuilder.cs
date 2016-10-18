using EasyBlock.Core.Interfaces.IO.Settings;
using NSubstitute;
using PeanutButter.RandomGenerators;
// ReSharper disable MemberCanBePrivate.Global

namespace EasyBlock.Core.Tests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SettingsBuilder: GenericBuilder<SettingsBuilder, ISettings>
    {
        public override ISettings ConstructEntity()
        {
            return Substitute.For<ISettings>();
        }

        public SettingsBuilder()
        {
            WithInterval(int.Parse(Constants.Defaults.ONE_DAY))
                .WithHostFile(Constants.Defaults.WINDOWS_HOSTS_FILE_LOCATION)
                .WithRedirectIp(Constants.Defaults.LOCALHOST)
                .WithSources(RandomValueGen.GetRandomHttpUrl(), RandomValueGen.GetRandomHttpUrl());
        }

        public SettingsBuilder WithHostFile(string path)
        {
            return WithProp(o => o.HostsFile.Returns(path));
        }

        public SettingsBuilder WithInterval(int interval)
        {
            return WithProp(o => o.RefreshIntervalInMinutes.Returns(interval));
        }

        public SettingsBuilder WithSources(params string[] sources)
        {
            return WithProp(o => o.Sources.Returns(sources));
        }

        public SettingsBuilder WithBlacklist(params string[] domains)
        {
            return WithProp(o => o.Blacklist.Returns(domains));
        }

        public SettingsBuilder WithRedirectIp(string address)
        {
            return WithProp(o => o.RedirectIp.Returns(address));
        }

        public SettingsBuilder WithWhitelist(params string[] regexes)
        {
            return WithProp(o => o.Whitelist.Returns(regexes));
        }
    }
}