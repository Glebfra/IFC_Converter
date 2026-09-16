using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class SegmentEntityExporter : IEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Segment;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            Segment segment = (Segment)entity;

            StartPipeEntity pipe = new StartPipeEntity();
            pipe.Diameter.CreateFromSI(segment.Diameter);

            FixedVector<Dim3> projection = segment.EndPort.Position - segment.StartPort.Position;
            pipe.ProjectionAlongOXAxis.CreateFromSI(projection[0]);
            pipe.ProjectionAlongOYAxis.CreateFromSI(projection[1]);
            pipe.ProjectionAlongOZAxis.CreateFromSI(projection[2]);

            context.Register(entity, pipe);
        }
    }
}