namespace EasyBlock.Core.Interfaces
{
    public interface IHostBlockCoordinator
    {
        void Apply();
        void Unapply();
    }
}