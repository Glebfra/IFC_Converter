using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Entities.Proxies
{
    internal sealed class VirtualPipeSegmentProxy : PipeSegmentProxy 
    {
        public VirtualPipeSegmentProxy(double diameter, double length, Vector<double> position, Vector<double> direction) 
            : base(diameter, length, position, direction)
        {
        }
    }
}