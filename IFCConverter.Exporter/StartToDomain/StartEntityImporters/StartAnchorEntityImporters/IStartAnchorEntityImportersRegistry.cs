using IFCConverter.Start.Entities.Anchors;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartAnchorEntityImporters
{
    internal interface IStartAnchorEntityImportersRegistry
    {
        IStartAnchorEntityImporter Resolve(StartAbstractAnchorEntity anchor);
        bool TryResolve(StartAbstractAnchorEntity anchor, out IStartAnchorEntityImporter importer);
    }
}