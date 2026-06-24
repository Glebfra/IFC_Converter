using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Topology
{
    internal sealed class TopologyModel : ITopologyModel
    {
        private readonly IReadOnlyDictionary<ITopologyEntity, IEnumerable<ITopologyEntity>> _connectedEntitiesMap;
        
        public TopologyModel(
            IReadOnlyCollection<ITopologyEntity> entities, 
            IReadOnlyCollection<ITopologyNodeEntity> nodes)
        {
            Entities = entities;
            Nodes = nodes;
        }

        public IReadOnlyCollection<ITopologyEntity> Entities { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }
        
        public IEnumerable<ITopologyEntity> GetConnectedEntities(ITopologyEntity entity)
        {
            return _connectedEntitiesMap[entity];
        }
    }
}