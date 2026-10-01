using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.TeeEntityImporters
{
    internal sealed class TeeEntityImportersRegistry : ReflectionRegistry<ITeeEntityImporter>, ITeeEntityImportersRegistry
    {
        public TeeEntityImportersRegistry() : base(typeof(TeeEntityImportersRegistry).Assembly)
        {
        }

        public ITeeEntityImporter Resolve(IIfcProduct product)
        {
            return Resolve(importer => importer.CanImport(product));
        }

        public bool TryResolve(IIfcProduct product, out ITeeEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product), out importer);
        }
    }
}