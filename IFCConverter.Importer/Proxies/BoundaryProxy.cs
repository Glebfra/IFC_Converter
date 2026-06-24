using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    internal class BoundaryProxy : IBoundaryProxy
    {
        public BoundaryProxy(IEntityProxy proxy, IReadOnlyCollection<Vector<double>> boundary)
        {
            Proxy = proxy;
            Boundary = boundary;
        }

        public IEntityProxy Proxy { get; set; }
        public IReadOnlyCollection<Vector<double>> Boundary { get; set; }
        
        public virtual IStartEntity ToStartEntity()
        {
            return Proxy.ToStartEntity();
        }
    }
}