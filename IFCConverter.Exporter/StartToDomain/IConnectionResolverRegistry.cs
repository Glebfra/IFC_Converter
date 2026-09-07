using IFCConverter.Exporter.StartToDomain.ConnectionResolvers;
using IFCConverter.Utils.Registries;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IConnectionResolverRegistry : IRegistry<IStartEntity, IConnectionResolver>
    {
    }
}