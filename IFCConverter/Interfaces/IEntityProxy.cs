using System.Diagnostics.Contracts;
using Start.Interfaces;

namespace IFCConverter.Interfaces
{
    internal interface IEntityProxy
    {
        [Pure]
        public IStartEntity ToStartEntity();
    }
}