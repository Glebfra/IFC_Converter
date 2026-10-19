using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Topology;
using IFCConverter.Importer.Extensions;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal sealed class ElbowMissingConnectionsAugmenter : IMissingConnectionsAugmenter
    {
        public bool CanAugment(Entity entity, EngineeringModel model, ExportContext context)
        {
            return entity is Elbow && entity.GetConnections(model).Count() != 2;
        }

        public void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
            Elbow elbow = (Elbow)entity;
            foreach (Port port in elbow.Ports)
            {
                Connection connection = port.GetConnections(model).FirstOrDefault();
                if (connection != null)
                    continue;

                Segment segment = SegmentCreator.CreateSegment(
                    elbow.Position, 
                    port.Position, 
                    port.Metadata.Diameter,
                    $"{elbow.Metadata.Name}_connection");
                model.Add(segment);
            }
        }
    }
}