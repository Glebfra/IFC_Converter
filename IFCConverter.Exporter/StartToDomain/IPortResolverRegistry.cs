using IFCConverter.Exporter.StartToDomain.PortResolvers;
using IFCConverter.Utils;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IPortResolverRegistry : IRegistry<IStartEntity, IPortResolver>
    {
    }
}