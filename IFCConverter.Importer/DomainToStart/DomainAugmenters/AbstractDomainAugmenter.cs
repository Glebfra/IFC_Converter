using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.DomainToStart.DomainAugmenters
{
    internal abstract class AbstractDomainAugmenter : IDomainAugmenter
    {
        public abstract bool CanAugment(Entity entity);
        public abstract void Augment(Entity entity, EngineeringModel model, ExportContext context);

        protected static Segment AddSegment(Port port, AbstractFitting fitting)
        {
            FixedVector<Dim3> projection = port.Position - fitting.Position;
            double length = projection.L2Norm();

            Segment segment = new Segment(EntityId.New())
            {
                Diameter = port.Metadata.Diameter
            };

            FixedVector<Dim3> startPos = fitting.Position;
            FixedVector<Dim3> endPos = startPos + length * port.Direction;

            FixedVector<Dim3>[] positions =
            {
                startPos, endPos
            };
            FixedVector<Dim3>[] directions =
            {
                port.Direction.Negate(), port.Direction
            };

            int i = 0;
            foreach (Port segmentPort in segment.Ports)
            {
                segmentPort.SetGeometry(positions[i], directions[i]);
                segmentPort.Metadata.Diameter = segment.Diameter;
                i++;
            }

            return segment;
        }
    }
}