using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;

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
        public string Name { get; set; }

        public IStartEntity ToStartEntity()
        {
            StartRestingSupportAnchorEntity entity = new StartRestingSupportAnchorEntity
            {
                Name = Name ?? string.Empty,
                Position = Position
            };
            return entity;
        }
    }
}