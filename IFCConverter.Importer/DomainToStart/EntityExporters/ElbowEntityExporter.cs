using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class ElbowEntityExporter : IEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Elbow;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            Elbow elbow = (Elbow)entity;

            StartElbowEntity startElbowEntity = new StartElbowEntity();
            startElbowEntity.Position = elbow.Position;
            startElbowEntity.Radius.CreateFromStart(elbow.Radius);

            context.Register(elbow, startElbowEntity);
        }
    }
}