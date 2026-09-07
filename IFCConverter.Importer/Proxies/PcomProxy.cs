using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    [ProxyEntity(2, typeof(PcomTopologyEntity), typeof(NearestSegmentBoundaryResolver))]
    internal sealed class PcomProxy : IFittingProxy
    {
        public PcomProxy(Vector<double> position)
        {
            Position = position;
        }

        public Vector<double> Position { get; }
        public string Name { get; set; }

        public IStartEntity ToStartEntity()
        {
            return new StartRigidElementEntity
            {
                Name = Name ?? string.Empty
            };
        }
    }
}