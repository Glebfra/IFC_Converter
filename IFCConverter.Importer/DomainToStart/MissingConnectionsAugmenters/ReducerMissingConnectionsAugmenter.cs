using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal sealed class ReducerMissingConnectionsAugmenter : IMissingConnectionsAugmenter
    {
        private const double MinSegmentLength = 0.01;
        
        public bool CanAugment(Entity entity, EngineeringModel model, ExportContext context)
        {
            return entity is Reducer && entity.GetConnections(model).Count() != 2;
        }

        public void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
            Reducer reducer = (Reducer)entity;
            Port[] reducerPorts = reducer.Ports.ToArray();
            
            foreach (Port reducerPort in reducerPorts)
            {
                Connection connection = reducerPort.GetConnections(model).FirstOrDefault();
                if (connection != null)
                    continue;
                
                FixedVector<Dim3> start = reducerPort.Position;
                FixedVector<Dim3> end = start + reducerPort.Direction * MinSegmentLength;
                double diameter = reducerPort.Metadata.Diameter;

                Segment segment = SegmentCreator.CreateSegment(
                    start,
                    end,
                    diameter,
                    $"{reducer.Metadata.Name}_connection");
                
                model.Add(segment);
            }
        }
    }
}