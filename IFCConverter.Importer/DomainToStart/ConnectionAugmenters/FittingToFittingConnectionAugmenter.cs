using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.DomainToStart.ConnectionAugmenters
{
    internal sealed class FittingToFittingConnectionAugmenter : IConnectionAugmenter
    {
        public bool CanAugment(Connection connection)
        {
            return connection.Type == ConnectionType.FittingToFitting;
        }

        public void Augment(Connection connection, EngineeringModel model, ExportContext context)
        {
            Port[] ports = connection.Ports.ToArray();
            FixedVector<Dim3> portPosition = ports.First().Position;
            double diameter = ports.Max(port => port.Metadata.Diameter);
            AbstractFitting[] fittings = ports.Select(port => model.GetEntity(port.Owner)).Cast<AbstractFitting>().ToArray();
            
            IEnumerable<Segment> segments;
            if (fittings.All(fitting => fitting is Reducer))
                segments = AugmentReducers(fittings.Cast<Reducer>().ToArray(), diameter, portPosition);
            else if (fittings.Any(fitting => fitting is Reducer))
                segments = AugmentReducer(fittings, diameter, portPosition);
            else
                segments = Augment(fittings, diameter);
            
            foreach (Segment segment in segments)
            {
                model.Add(segment);
            }
        }

        private static IEnumerable<Segment> AugmentReducers(Reducer[] reducers, double diameter, FixedVector<Dim3> portPosition)
        {
            foreach (Reducer reducer in reducers)
            {
                if (reducer.Position.AlmostEqual(portPosition))
                    continue;

                FixedVector<Dim3> reducerAxis = reducer.TypedMetadata.Axis;

                yield return SegmentCreator.CreateSegment(
                    portPosition,
                    reducer.Position,
                    diameter,
                    $"{reducer.Metadata.Name}_connection",
                    (reducer.Position - portPosition).Dot(reducerAxis) * reducerAxis
                );
            }
        }

        private static IEnumerable<Segment> AugmentReducer(AbstractFitting[] fittings, double diameter, FixedVector<Dim3> portPosition)
        {
            Reducer reducer = (Reducer)fittings.First(fitting => fitting is Reducer);
            AbstractFitting abstractFitting = fittings.First(fitting => fitting != reducer);

            FixedVector<Dim3> reducerAxis = reducer.TypedMetadata.Axis;
            
            yield return SegmentCreator.CreateSegment(
                abstractFitting.Position, 
                reducer.Position, 
                diameter, 
                $"{fittings[0].Metadata.Name}_{fittings[1].Metadata.Name}_connection",
                (reducer.Position - abstractFitting.Position).Dot(reducerAxis) * reducerAxis
            );
        }

        private static IEnumerable<Segment> Augment(AbstractFitting[] fittings, double diameter)
        {
            FixedVector<Dim3>[] fittingPositions = fittings.Select(fitting => fitting.Position).ToArray();
            yield return SegmentCreator.CreateSegment(fittingPositions[0], fittingPositions[1], diameter, $"{fittings[0].Metadata.Name}_{fittings[1].Metadata.Name}_connection");
        }
    }
}