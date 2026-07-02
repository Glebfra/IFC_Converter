using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Anchors;
using Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    [ProxyEntity(0, typeof(AttachmentTopologyEntity), typeof(AttachmentBoundaryResolver), typeof(PointInSegmentConnectionResolver))]
    internal sealed class AttachmentProxy : IFittingProxy
    {
        public AttachmentProxy(Vector<double> position)
        {
            Position = position;
        }

        public Vector<double> Position { get; }
        public string? Name { get; set; }

        public IStartEntity ToStartEntity()
        {
            StartRestingSupportAnchorEntity entity = new()
            {
                Name = Name ?? string.Empty,
                Position = Position
            };
            return entity;
        }
    }
}