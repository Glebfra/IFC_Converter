using IFCConverter.Domain.Entities;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal interface IJointDomainEntityExportersRegistry
    {
        bool TryResolve(Joint joint, out IJointDomainEntityExporter exporter);
    }
}