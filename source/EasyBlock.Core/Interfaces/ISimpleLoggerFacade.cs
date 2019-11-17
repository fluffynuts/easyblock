using PeanutButter.ServiceShell;

namespace EasyBlock.Core.Interfaces
{
    public interface ISimpleLoggerFacade: ISimpleLogger
    {
        void SetLogger(ISimpleLogger logger);
    }
}