using IFCConverter.Domain;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.ConnectionResolvers
{
    internal interface IConnectionResolver
    {
        bool CanResolve(IStartEntity source);
        void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context);
    }
}