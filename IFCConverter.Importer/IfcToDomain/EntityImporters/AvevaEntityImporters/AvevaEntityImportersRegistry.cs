using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters
{
    internal sealed class AvevaEntityImportersRegistry : ReflectionRegistry<IAvevaEntityImporter>, IAvevaEntityImportersRegistry
    {
        public AvevaEntityImportersRegistry() : base(typeof(AvevaEntityImportersRegistry).Assembly)
        {
        }

        public IAvevaEntityImporter Resolve(IIfcProduct product, AvevaEntityType type)
        {
            return Resolve(importer => importer.CanImport(product, type));
        }

        public bool TryResolve(IIfcProduct product, AvevaEntityType type, out IAvevaEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product, type), out importer);
        }
    }
}