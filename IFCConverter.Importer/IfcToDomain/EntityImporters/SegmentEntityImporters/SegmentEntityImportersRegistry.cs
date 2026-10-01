using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.SegmentEntityImporters
{
    internal sealed class SegmentEntityImportersRegistry : ReflectionRegistry<ISegmentEntityImporter>, ISegmentEntityImportersRegistry
    {
        public SegmentEntityImportersRegistry() : base(typeof(SegmentEntityImportersRegistry).Assembly)
        {
        }

        public ISegmentEntityImporter Resolve(IIfcProduct product)
        {
            return Resolve(importer => importer.CanImport(product));
        }

        public bool TryResolve(IIfcProduct product, out ISegmentEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product), out importer);
        }
    }
}