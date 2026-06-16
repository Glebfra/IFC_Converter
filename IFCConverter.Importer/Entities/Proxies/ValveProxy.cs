using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Importer.Entities.Proxies
{
    [ProxyEntity(typeof(NearestSegmentBoundaryResolver), 2)]
    internal sealed class ValveProxy : IFittingProxy
    {
        public ValveProxy(Vector<double> position)
        {
            Position = position;
        }

        public Vector<double> Position { get; }
        public string? Name { get; set; }
        
        public IStartEntity ToStartEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}