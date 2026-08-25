using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    [DomainToIfcPhase(1)]
    public sealed class EntityExportPhase : IDomainToIfcPhase
    {
        private readonly IDomainEntityExporterRegistry _domainEntityExporterRegistry = new DomainEntityExporterRegistry();
        
        public void Execute(EngineeringModel domain, IModel model, ExportContext context)
        {
            foreach (Entity domainEntity in domain.Entities)
            {
                if (_domainEntityExporterRegistry.TryResolve(domainEntity, out IDomainEntityExporter exporter))
                    exporter.Export(domainEntity, model, context);
            }
        }
    }
}