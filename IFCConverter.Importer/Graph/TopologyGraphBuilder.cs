using System.Collections.Generic;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Graph
{
    internal sealed class TopologyGraphBuilder
    {
        private readonly VectorComparer _comparer;

        public TopologyGraphBuilder(VectorComparer comparer)
        {
            _comparer = comparer;
        }

        public TopologyGraph Build(IEnumerable<IFittingProxy> fittings, IEnumerable<IResolvedSegmentProxy> segments)
        {
            TopologyGraph graph = new TopologyGraph(_comparer);

            foreach (IFittingProxy fitting in fittings)
            {
                ConnectionNode node = graph.GetOrCreateNode(fitting.Position);
                node.Fittings.Add(fitting);
            }
            
            foreach (IResolvedSegmentProxy segment in segments)
            {
                ConnectionNode startNode = graph.GetOrCreateNode(segment.ResolvedStartPosition);
                ConnectionNode endNode = graph.GetOrCreateNode(segment.ResolvedEndPosition);

                startNode.Segments.Add(segment);
                endNode.Segments.Add(segment);
            }

            return graph;
        }
    }
}