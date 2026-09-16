using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters
{
    internal sealed class UnknownEntityImportersRegistry : ReflectionRegistry<IUnknownEntityImporter>, IUnknownEntityImportersRegistry
    {
        public UnknownEntityImportersRegistry() : base(typeof(UnknownEntityImporter).Assembly)
        {
        }

        public IUnknownEntityImporter Resolve(IIfcProduct product)
        {
            return Resolve(importer => importer.CanImport(product));
        }

        public bool TryResolve(IIfcProduct product, out IUnknownEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product), out importer);
        }
    }
}