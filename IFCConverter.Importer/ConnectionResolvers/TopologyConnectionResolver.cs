using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.ConnectionResolvers
{
    internal sealed class TopologyConnectionResolver
    {
        public IEnumerable<ITopologyEntity> GetConnectedEntities(ITopologyEntity entity, IEnumerable<ITopologyEntity> allTopologies)
        {
            List<ITopologyEntity> result = new();
            foreach (ITopologyEntity otherTopology in allTopologies)
            {
                if (entity.Equals(otherTopology))
                    continue;

                if (entity.Connected.Contains(otherTopology))
                {
                    result.Add(otherTopology);
                    continue;
                }

                bool isConnected = entity.Nodes.Any(n1 => otherTopology.Nodes.Any(n2 => n1.Equals(n2)));
                if (isConnected)
                    result.Add(otherTopology);
            }

            return result;
        }
    }
}