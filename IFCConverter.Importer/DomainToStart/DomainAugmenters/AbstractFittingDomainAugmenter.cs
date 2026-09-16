using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Importer.DomainToStart.DomainAugmenters
{
    internal sealed class AbstractFittingDomainAugmenter : AbstractDomainAugmenter
    {
        public override bool CanAugment(Entity entity)
        {
            return entity is AbstractFitting &&
                   !(entity is Reducer);
        }

        public override void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
            AbstractFitting fitting = (AbstractFitting)entity;

            IReadOnlyCollection<Connection> connections = model.GetConnections(entity.Id).ToArray();
            if (connections.Count == entity.Ports.Count)
                return;

            foreach (Port port in entity.Ports)
            {
                IEnumerable<Connection> portConnections = model.GetConnections(port.Id);
                if (portConnections.Any())
                    continue;

                Segment segment = AddSegment(port, fitting);
                model.Add(segment);
            }
        }
    }
}