using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.TeeEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class TeeEntityImporter : IIfcEntityImporter
    {
        private readonly ITeeEntityImportersRegistry _registry = new TeeEntityImportersRegistry();
        
        public bool CanImport(EntityType type)
        {
            return type == EntityType.TEE;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, out ITeeEntityImporter importer))
                importer.Import(product, model, context);
        }
    }
}