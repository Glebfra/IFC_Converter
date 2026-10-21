using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class ValveEntityImporter : IIfcPostEntityImporter
    {
        private readonly IValveEntityImportersRegistry _registry = new ValveEntityImportersRegistry();
        
        public bool CanImport(EntityType type)
        {
            return type == EntityType.VALVE;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, model, context, out IValveEntityImporter importer))
                importer.Import(product, model, context);
        }
    }
}