using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters.UnknownSegmentEntityImporters
{
    internal sealed class UnknownSegmentEntityImportersRegistry : ReflectionRegistry<IUnknownSegmentEntityImporter>, IUnknownSegmentEntityImportersRegistry
    {
        public UnknownSegmentEntityImportersRegistry() : base(typeof(UnknownSegmentEntityImportersRegistry).Assembly)
        {
        }

        public IUnknownSegmentEntityImporter Resolve(IIfcPipeSegment segment)
        {
            return Resolve(importer => importer.CanImport(segment));
        }

        public bool TryResolve(IIfcPipeSegment segment, out IUnknownSegmentEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(segment), out importer);
        }
    }
}