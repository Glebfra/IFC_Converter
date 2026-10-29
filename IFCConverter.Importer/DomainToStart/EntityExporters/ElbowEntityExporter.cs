using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class ElbowEntityExporter : IEntityExporter
    {
        private Elbow _elbow;

        public bool CanExport(Entity entity)
        {
            if (!(entity is Elbow elbow))
                return false;
            _elbow = elbow;

            return true;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            StartElbowEntity startElbowEntity = new StartElbowEntity
            {
                Position = _elbow.Position,
                Name = _elbow.Metadata.Name
            };
            startElbowEntity.Radius.CreateFromStart(_elbow.Radius);

            context.Register(_elbow, startElbowEntity);
        }
    }
}