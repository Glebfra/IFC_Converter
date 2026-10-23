using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AnchorEntityImporters
{
    internal sealed class AnchorEntityImportersRegistry : ReflectionRegistry<IAnchorEntityImporter>, IAnchorEntityImportersRegistry
    {
        public AnchorEntityImportersRegistry() : base(typeof(AnchorEntityImportersRegistry).Assembly)
        {
        }

        public IAnchorEntityImporter Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(importer => importer.CanImport(product, model, context));
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IAnchorEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product, model, context), out importer);
        }
    }
}