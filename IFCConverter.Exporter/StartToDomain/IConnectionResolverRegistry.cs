using IFCConverter.Exporter.StartToDomain.ConnectionResolvers;
using IFCConverter.Utils;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IConnectionResolverRegistry : IRegistry<IStartEntity, IConnectionResolver>
    {
    }
}