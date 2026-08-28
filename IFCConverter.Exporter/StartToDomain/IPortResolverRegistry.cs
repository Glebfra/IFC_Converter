using IFCConverter.Exporter.StartToDomain.PortResolvers;
using IFCConverter.Utils.Registries;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IPortResolverRegistry : IRegistry<IStartEntity, IPortResolver>
    {
    }
}