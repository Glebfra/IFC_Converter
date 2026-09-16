using IFCConverter.Domain;
using IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters
{
    internal sealed class UnknownEntityImporter : IIfcEntityImporter
    {
        private readonly IUnknownEntityImportersRegistry _registry = new UnknownEntityImportersRegistry();
        
        public bool CanImport(IIfcProduct product, ImportContext context)
        {
            return context.ImportType == ImportType.UNKNOWN;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (_registry.TryResolve(product, out IUnknownEntityImporter importer))
                importer.Import(product, model, context);
        }
    }
}