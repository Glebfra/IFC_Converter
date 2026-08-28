using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Graph
{
    internal sealed class ConnectionNode
    {
        public ConnectionNode(Vector<double> position, IEqualityComparer<IResolvedSegmentProxy> segmentComparer = null)
        {
            Position = position;
            Fittings = new HashSet<IFittingProxy>();
            Segments = segmentComparer != null
                ? new HashSet<IResolvedSegmentProxy>(segmentComparer)
                : new HashSet<IResolvedSegmentProxy>();
        }

        public Vector<double> Position { get; }
        public HashSet<IFittingProxy> Fittings { get; }
        public HashSet<IResolvedSegmentProxy> Segments { get; }
    }
}