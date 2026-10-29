using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Anchors;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class AnchorEntityExporter : IEntityExporter
    {
        private Anchor _anchor;

        public bool CanExport(Entity entity)
        {
            if (!(entity is Anchor anchor))
                return false;
            _anchor = anchor;

            return true;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            StartMomentFreeAnchorEntity anchorEntity = new StartMomentFreeAnchorEntity
            {
                Position = _anchor.Position,
                Name = entity.Metadata.Name
            };

            context.Register(entity, anchorEntity);
        }
    }
}