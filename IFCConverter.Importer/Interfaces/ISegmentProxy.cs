using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ISegmentProxy : IEntityProxy
    {
        double Diameter { get; }
        double Length { get; }
        Vector<double> Direction { get; }
        Vector<double> EndPosition { get; }
    }
}