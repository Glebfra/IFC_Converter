using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.ElbowEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class ElbowEntityImporter : IIfcEntityImporter
    {
        private readonly IElbowEntityImportersRegistry _registry = new ElbowEntityImportersRegistry();
        
        public bool CanImport(EntityType type)
        {
            return type == EntityType.ELBOW;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, out IElbowEntityImporter importer))
                importer.Import(product, model, context);
        }
    }
}