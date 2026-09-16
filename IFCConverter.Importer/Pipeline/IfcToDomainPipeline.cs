using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain;
using IFCConverter.Importer.IfcToDomain.Phases;
using Xbim.Common;

namespace IFCConverter.Importer.Pipeline
{
    internal sealed class IfcToDomainPipeline
    {
        private readonly IfcToDomainPhaseRegistry _registry = new IfcToDomainPhaseRegistry();

        public EngineeringModel Execute(IModel model)
        {
            ImportType type = ResolveType(model);
            
            EngineeringModel domain = new EngineeringModel();
            ImportContext context = new ImportContext(type);

            foreach (IIfcToDomainPhase ifcToDomainPhase in _registry.GetPhases())
                ifcToDomainPhase.Execute(model, domain, context);

            return domain;
        }
        
        private static ImportType ResolveType(IModel model)
        {
            if (model.Header.CreatingApplication.Contains("AVEVA E3D"))
                return ImportType.AVEVA;

            return ImportType.UNKNOWN;
        }
    }
}