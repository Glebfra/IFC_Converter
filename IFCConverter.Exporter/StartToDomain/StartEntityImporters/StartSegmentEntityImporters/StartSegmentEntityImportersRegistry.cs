using IFCConverter.Start.Entities.Segments;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartSegmentEntityImporters
{
    internal sealed class StartSegmentEntityImportersRegistry : ReflectionRegistry<IStartSegmentEntityImporter>, IStartSegmentEntityImportersRegistry
    {
        public StartSegmentEntityImportersRegistry() : base(typeof(StartSegmentEntityImportersRegistry).Assembly)
        {
        }

        public IStartSegmentEntityImporter Resolve(StartAbstractSegmentEntity segment)
        {
            return Resolve(importer => importer.CanImport(segment));
        }

        public bool TryResolve(StartAbstractSegmentEntity segment, out IStartSegmentEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(segment), out importer);
        }
    }
}