using System.Diagnostics.Contracts;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    [ProxyEntity(2, typeof(ValveTopologyEntity), typeof(ValveConnectionAugmenter), typeof(NearestSegmentBoundaryResolver))]
    internal sealed class ValveProxy : IFittingProxy
    {
        public ValveProxy(Vector<double> position)
        {
            Position = position;
        }

        public Vector<double> Position { get; set; }
        public string? Name { get; set; }

        [Pure]
        public IStartEntity ToStartEntity()
        {
            StartValveEntity valveEntity = new()
            {
                Position = Position,
                Name = Name ?? string.Empty
            };

            return valveEntity;
        }
    }
}