using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Importer.DomainToStart.DomainAugmenters
{
    internal sealed class ReducerDomainAugmenter : AbstractDomainAugmenter
    {
        public override bool CanAugment(Entity entity)
        {
            return entity is Reducer;
        }

        public override void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
            Reducer reducer = (Reducer)entity;

            IReadOnlyCollection<Connection> connections = model.GetConnections(entity.Id).ToArray();
            if (connections.Count == entity.Ports.Count)
                return;

            IEnumerable<Connection> portConnection = model.GetConnections(reducer.PortB.Id);
            if (portConnection.Any())
                return;

            Segment segment = AddSegment(reducer.PortB, reducer);
            model.Add(segment);
        }
    }
}