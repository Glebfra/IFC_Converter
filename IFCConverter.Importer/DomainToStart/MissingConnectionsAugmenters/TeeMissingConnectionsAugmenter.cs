using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Topology;
using IFCConverter.Importer.Extensions;

namespace IFCConverter.Importer.DomainToStart.MissingConnectionsAugmenters
{
    internal sealed class TeeMissingConnectionsAugmenter : IMissingConnectionsAugmenter
    {
        public bool CanAugment(Entity entity, EngineeringModel model, ExportContext context)
        {
            return entity is Tee && entity.GetConnections(model).Count() != 3;
        }

        public void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
            Tee tee = (Tee)entity;
            foreach (Port port in tee.Ports)
            {
                Connection connection = port.GetConnections(model).FirstOrDefault();
                if (connection != null)
                    continue;

                Segment segment = SegmentCreator.CreateSegment(
                    tee.Position, 
                    port.Position, 
                    port.Metadata.Diameter,
                    $"{tee.Metadata.Name}_connection");
                model.Add(segment);
            }
        }
    }
}