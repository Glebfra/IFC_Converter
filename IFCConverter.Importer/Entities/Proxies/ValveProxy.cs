using System.Diagnostics.Contracts;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Entities.Proxies
{
    [ProxyEntity(2, typeof(ValveConnectionAugmenter), typeof(NearestSegmentBoundaryResolver))]
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
            StartValveEntity valveEntity = new StartValveEntity
            {
                Position = Position,
                Name = Name ?? string.Empty
            };

            return valveEntity;
        }
    }
}