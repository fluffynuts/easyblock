using PeanutButter.ServiceShell;

namespace EasyBlock.Core.Interfaces.IO
{
    public interface ISimpleLoggerFacade: ISimpleLogger
    {
        void SetLogger(ISimpleLogger logger);
    }
}