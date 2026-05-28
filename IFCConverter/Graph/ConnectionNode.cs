using System.Collections.Generic;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Graph
{
    internal sealed class ConnectionNode
    {
        public Vector<double> Position { get; }
        public HashSet<IFittingProxy> Fittings { get; }
        public HashSet<IResolvedSegmentProxy> Segments { get; }

        public ConnectionNode(Vector<double> position, IEqualityComparer<IResolvedSegmentProxy>? segmentComparer = null)
        {
            Position = position;
            Fittings = new HashSet<IFittingProxy>();
            Segments = segmentComparer != null
                ? new HashSet<IResolvedSegmentProxy>(segmentComparer)
                : new HashSet<IResolvedSegmentProxy>();
        }
    }
}