using IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ReducerEntityImporters
{
    internal sealed class ReducerEntityImportersRegistry : ReflectionRegistry<IReducerEntityImporter>, IReducerEntityImportersRegistry
    {
        public ReducerEntityImportersRegistry() : base(typeof(ReducerEntityImportersRegistry).Assembly)
        {
        }

        public IReducerEntityImporter Resolve(IIfcProduct product)
        {
            return Resolve(importer => importer.CanImport(product));
            throw new System.NotImplementedException();
        }

        public bool TryResolve(IIfcProduct product, out IReducerEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product), out importer);
        }
    }
}