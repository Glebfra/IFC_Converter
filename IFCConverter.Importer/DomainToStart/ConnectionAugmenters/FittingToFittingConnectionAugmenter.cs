using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
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
            AbstractFitting[] fittings = ports.Select(port => model.GetEntity(port.Owner)).Cast<AbstractFitting>().ToArray();
            
            double diameter = ports.Max(port => port.Metadata.Diameter);
            
            IEnumerable<Segment> segments;
            if (fittings.Any(fitting => fitting is Reducer))
                segments = AugmentReducer(fittings, diameter, ports.First());
            else
                segments = Augment(fittings, diameter);
            
            foreach (Segment segment in segments)
            {
                model.Add(segment);
            }
        }

        private static IEnumerable<Segment> AugmentReducer(AbstractFitting[] fittings, double diameter, Port port)
        {
            List<Segment> segments = new List<Segment>();
            
            Reducer reducer = (Reducer)fittings.First(fitting => fitting is Reducer);
            AbstractFitting abstractFitting = fittings.First(fitting => !(fitting is Reducer));

            if (reducer.Position.AlmostEqual(port.Position))
                return new Segment[] { SegmentCreator.CreateSegment(abstractFitting.Position, reducer.Position, diameter) };

            FixedVector<Dim3> reducerAxis = (FixedVector<Dim3>)reducer.Metadata.Meta["Axis"];
            
            segments.Add(SegmentCreator.CreateSegment(
                reducer.Position, 
                port.Position, 
                diameter, 
                $"{fittings[0].Metadata.Name}_{fittings[1].Metadata.Name}_connection",
                (port.Position - reducer.Position).Dot(reducerAxis) * reducerAxis
            ));

            segments.Add(SegmentCreator.CreateSegment(
                abstractFitting.Position,
                port.Position,
                diameter,
                $"{fittings[0].Metadata.Name}_{fittings[1].Metadata.Name}_connection"
            ));
            
            return segments;
        }

        private static IEnumerable<Segment> Augment(AbstractFitting[] fittings, double diameter)
        {
            FixedVector<Dim3>[] fittingPositions = fittings.Select(fitting => fitting.Position).ToArray();
            yield return SegmentCreator.CreateSegment(fittingPositions[0], fittingPositions[1], diameter, $"{fittings[0].Metadata.Name}_{fittings[1].Metadata.Name}_connection");
        }
    }
}