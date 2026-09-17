using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain;
using IFCConverter.Importer.IfcToDomain.Phases;
using Xbim.Common;

namespace IFCConverter.Importer.Pipeline
{
    internal sealed class IfcToDomainPipeline
    {
        private readonly IfcToDomainPhaseRegistry _registry = new IfcToDomainPhaseRegistry();
        private readonly ImportTypeResolver _importTypeResolver = new ImportTypeResolver();

        public EngineeringModel Execute(IModel model)
        {
            ImportType type = _importTypeResolver.ResolveImportType(model);

            EngineeringModel domain = new EngineeringModel();
            ImportContext context = new ImportContext(type);

            foreach (IIfcToDomainPhase ifcToDomainPhase in _registry.GetPhases())
                ifcToDomainPhase.Execute(model, domain, context);

            return domain;
        }
    }
}