using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.ReducerEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class ReducerEntityImporter : IIfcEntityImporter
    {
        private readonly IReducerEntityImportersRegistry _registry = new ReducerEntityImportersRegistry();

        public bool CanImport(EntityType type)
        {
            return type == EntityType.REDUCER;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, out IReducerEntityImporter importer))
                importer.Import(product, model, context);
        }
    }
}