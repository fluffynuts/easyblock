using System;
using System.Diagnostics;
using Castle.Windsor;
using EasyBlock.Core;
using EasyBlock.Core.Interfaces;
using EasyBlock.Core.Interfaces.Settings;
using PeanutButter.ServiceShell;

namespace EasyBlock.Win32Service
{
    public class EasyBlockService: Shell
    {
        public IWindsorContainer WindsorContainer => _container;
        private readonly IWindsorContainer _container;

        public EasyBlockService(): this(WindsorBootstrapper.Bootstrap())
        {
        }

        public EasyBlockService(IWindsorContainer container)
        {
            _container = container;
            DisplayName = "EasyBlock AdBlocker";
            ServiceName = "EasyBlock";
            Interval = _container.Resolve<ISettings>().RefreshIntervalInMinutes * 60;
        }

        protected override void RunOnce()
        {
            var logger = SetupLogging();
            var coordinator = _container.Resolve<IHostBlockCoordinator>();
            try
            {
                coordinator.Apply();
            }
            catch (Exception ex)
            {
                logger.LogFatal($"Exception whilst applying block lists: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private ISimpleLoggerFacade SetupLogging()
        {
            var loggerFacade = _container.Resolve<ISimpleLoggerFacade>();
            loggerFacade.SetLogger(this);
            return loggerFacade;
        }

        protected override void OnStop()
        {
            var logger = SetupLogging();
            try
            {
                logger.LogInfo("Unapplying blocklists...");
                var coordinator = _container.Resolve<IHostBlockCoordinator>();
                coordinator.Unapply();
            }
            catch (Exception ex)
            {
                logger.LogFatal($"Unable to Unapply blocklists: {ex.Message}");
            }
            finally
            {
                base.OnStop();
            }
        }
    }
}