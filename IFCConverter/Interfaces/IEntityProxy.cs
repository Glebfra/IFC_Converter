using Start.Interfaces;

namespace IFCConverter.Interfaces
{
    public interface IEntityProxy
    {
        public IStartEntity ToStartEntity();
    }
}