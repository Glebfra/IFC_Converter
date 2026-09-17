using IFCConverter.Exporter.StartToDomain.PortResolvers;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IPortResolverRegistry : IRegistry<IStartEntity, IPortResolver>
    {
    }
}