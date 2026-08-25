using IFCConverter.Domain;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal interface IPortResolver
    {
        bool CanResolve(IStartEntity source);
        void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context);
    }
}