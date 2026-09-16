using IFCConverter.Exporter.StartToDomain.ConnectionResolvers;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IConnectionResolverRegistry : IRegistry<IStartEntity, IConnectionResolver>
    {
    }
}