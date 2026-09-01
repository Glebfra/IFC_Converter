using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal interface IJointDomainEntityExportersRegistry
    {
        IJointDomainEntityExporter Resolve(Joint joint);
        bool TryResolve(Joint joint, out IJointDomainEntityExporter exporter);
    }
}