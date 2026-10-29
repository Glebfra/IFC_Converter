using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.Attributes;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters;
using Microsoft.Extensions.Logging;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.Phases
{
    [DomainToIfcPhase(1)]
    public sealed class EntityExportPhase : IDomainToIfcPhase
    {
        private readonly IDomainEntityExporterRegistry _domainEntityExporterRegistry = new DomainEntityExporterRegistry();

        public void Execute(EngineeringModel domain, IModel model, ExportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<EntityExportPhase>();

            foreach (Entity domainEntity in domain.Entities)
            {
                if (_domainEntityExporterRegistry.TryResolve(domainEntity, out IDomainEntityExporter exporter))
                {
                    logger.LogInformation(
                        "Found domain entity exporter {exporter} for domain entity {entity} with id {id}",
                        exporter.GetType().Name,
                        domainEntity.GetType().Name,
                        domainEntity.Id
                    );
                    exporter.Export(domainEntity, model, context);
                }
            }
        }
    }
}