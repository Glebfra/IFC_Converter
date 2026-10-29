using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.IfcToDomain.SegmentSplitters
{
    internal sealed class AnchorSegmentSplitter : ISegmentSplitter
    {
        private Anchor _anchor;

        public bool CanSplit(Entity entity, EngineeringModel model, ImportContext context)
        {
            if (!(entity is Anchor anchor))
                return false;
            _anchor = anchor;

            return true;
        }

        public void Split(Entity entity, EngineeringModel model, ImportContext context)
        {
            FixedVector<Dim3> position = _anchor.Position;
            AbstractSegment segment = FindNearestSegment(position, model);
            if (segment == null || segment.Ports.Any(port => port.Position.AlmostEqual(position)))
                return;

            double diameter = segment.StartPort.Metadata.Diameter;

            FixedVector<Dim3> endPos = segment.EndPort.Position;
            FixedVector<Dim3> endDir = segment.EndPort.Direction;

            segment.EndPort.SetGeometry(position, endDir);

            AbstractSegment newSegment = SegmentCreator.CreateSegment(
                position,
                endPos,
                diameter,
                $"{segment.Metadata.Name}_splitted"
            );

            model.Add(newSegment);
        }

        private static AbstractSegment FindNearestSegment(FixedVector<Dim3> position, EngineeringModel model)
        {
            foreach (AbstractSegment segment in model.Entities.OfType<AbstractSegment>())
            {
                if (segment.Ports.Any(port => port.Position.AlmostEqual(position)))
                    return segment;

                if (position.IsBetween(segment.StartPort.Position, segment.EndPort.Position, 1e-3))
                    return segment;
            }

            return null;
        }
    }
}