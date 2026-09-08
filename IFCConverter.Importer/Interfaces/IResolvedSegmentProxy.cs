using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IResolvedSegmentProxy : IResolvedProxy
    {
        Vector<double> ResolvedStartPosition { get; }
        Vector<double> ResolvedEndPosition { get; }

        Vector<double> ResolvedProjection { get; }

        IStartSegmentEntity ToStartEntity();
    }
}