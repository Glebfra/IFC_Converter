using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Topology
{
    internal sealed class ValveTopologyEntity : TopologyEntity
    {
        public ValveTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes,
            IReadOnlyCollection<IBoundaryProxy> connectedProxies)
            : base(proxy, nodes, connectedProxies)
        {
        }

        public double Length => GetLength();

        public override IStartEntity ToStartEntity()
        {
            StartValveEntity valveEntity = (StartValveEntity)base.ToStartEntity();
            valveEntity.Length.CreateFromSI(Length);
            return valveEntity;
        }

        private double GetLength()
        {
            return (Proxy.Boundary.ElementAt(1) - Proxy.Boundary.ElementAt(0)).L2Norm();
        }
    }
}