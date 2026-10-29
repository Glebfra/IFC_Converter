using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain;
using IFCConverter.Importer.IfcToDomain.Phases;
using Microsoft.Extensions.Logging;
using Xbim.Common;

namespace IFCConverter.Importer.Pipeline
{
    internal sealed class IfcToDomainPipeline
    {
        private readonly IfcToDomainPhaseRegistry _registry = new IfcToDomainPhaseRegistry();
        private readonly ImportTypeResolver _importTypeResolver = new ImportTypeResolver();

        public EngineeringModel Execute(IModel model, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<IfcToDomainPipeline>();
            
            ImportType type = _importTypeResolver.ResolveImportType(model);
            logger.LogInformation("Resolved import type: {type}", type.ToString());

            EngineeringModel domain = new EngineeringModel();
            ImportContext context = new ImportContext(type);

            foreach (IIfcToDomainPhase ifcToDomainPhase in _registry.GetPhases())
            {
                logger.LogInformation("Executing import phase: {phase}", ifcToDomainPhase.GetType().Name);
                ifcToDomainPhase.Execute(model, domain, context, factory);
            }

            return domain;
        }
    }
}