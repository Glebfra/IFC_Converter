using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class SegmentEntityExporter : IEntityExporter
    {
        private Segment _segment;
        
        public bool CanExport(Entity entity)
        {
            if (!(entity is Segment segment))
                return false;
            _segment = segment;

            return true;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            StartPipeEntity pipe = new StartPipeEntity();
            pipe.Diameter.CreateFromSI(_segment.Diameter);

            FixedVector<Dim3> projection = _segment.TypedMetadata.Projection ?? 
                                           _segment.EndPort.Position - _segment.StartPort.Position;
            pipe.ProjectionAlongOXAxis.CreateFromSI(projection[0]);
            pipe.ProjectionAlongOYAxis.CreateFromSI(projection[1]);
            pipe.ProjectionAlongOZAxis.CreateFromSI(projection[2]);
            pipe.Name = _segment.Metadata.Name;

            context.Register(entity, pipe);
        }
    }
}