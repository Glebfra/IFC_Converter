using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.DomainToStart.ConnectionAugmenters
{
    internal sealed class PipeToFittingConnectionAugmenter : IConnectionAugmenter
    {
        public bool CanAugment(Connection connection)
        {
            return connection.Type == ConnectionType.PipeToFitting;
        }

        public void Augment(Connection connection, EngineeringModel model, ExportContext context)
        {
            Port[] ports = connection.Ports.ToArray();
            Entity[] entities = ports.Select(port => model.GetEntity(port.Owner)).ToArray();
            
            AbstractSegment segment;
            AbstractFitting fitting;
            if (entities[0] is Segment)
            {
                segment = (AbstractSegment)entities[0];
                fitting = (AbstractFitting)entities[1];
            }
            else
            {
                fitting = (AbstractFitting)entities[0];
                segment = (AbstractSegment)entities[1];
            }

            if (fitting is Reducer reducer)
            {
                double diameter = ports.Max(port => port.Metadata.Diameter);
                AugmentReducer(model, reducer, ports.First().Position, diameter);
                return;
            }

            Augment(fitting, segment);
        }

        private static void AugmentReducer(EngineeringModel model, Reducer reducer, FixedVector<Dim3> portPosition, double diameter)
        {
            if (reducer.Position.AlmostEqual(portPosition))
                return;
            
            FixedVector<Dim3> reducerAxis = reducer.TypedMetadata.Axis;

            Segment segment = SegmentCreator.CreateSegment(
                portPosition,
                reducer.Position,
                diameter,
                $"{reducer.Metadata.Name}_connection",
                (reducer.Position - portPosition).Dot(reducerAxis) * reducerAxis
            );
            model.Add(segment);
        }

        private static void Augment(AbstractFitting fitting, AbstractSegment segment)
        {
            Port nearestSegmentPort = segment.GetNearestPort(fitting.Position);
            nearestSegmentPort.SetGeometry(fitting.Position, nearestSegmentPort.Direction);
        }
    }
}