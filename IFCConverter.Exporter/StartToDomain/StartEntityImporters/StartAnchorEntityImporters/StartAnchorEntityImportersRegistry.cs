using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartAnchorEntityImporters
{
    internal sealed class StartAnchorEntityImportersRegistry : ReflectionRegistry<IStartAnchorEntityImporter>, IStartAnchorEntityImportersRegistry
    {
        public StartAnchorEntityImportersRegistry() : base(typeof(StartAnchorEntityImportersRegistry).Assembly)
        {
        }

        public IStartAnchorEntityImporter Resolve(StartAbstractAnchorEntity anchor)
        {
            return Resolve(importer => importer.CanImport(anchor));
        }

        public bool TryResolve(StartAbstractAnchorEntity anchor, out IStartAnchorEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(anchor), out importer);
        }
    }
}