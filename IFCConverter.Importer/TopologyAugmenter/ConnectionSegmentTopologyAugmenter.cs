using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using Utils;

namespace IFCConverter.Importer.TopologyAugmenter
{
    internal sealed class ConnectionSegmentTopologyModelAugmenter : ITopologyModelAugmenter
    {
        private readonly ConnectionAugmenter _connectionAugmenter = new();
        private readonly TopologyModelBuilder _modelBuilder;

        public ConnectionSegmentTopologyModelAugmenter(VectorComparer comparer)
        {
            _modelBuilder = new TopologyModelBuilder(comparer);
        }

        public ITopologyModel Augment(ITopologyModel model)
        {
            IReadOnlyCollection<ISegmentProxy> generatedSegments = model.Entities
                .SelectMany(_connectionAugmenter.Augment)
                .ToArray();

            if (generatedSegments.Count == 0)
                return model;

            List<IEntityProxy> proxies = model.Entities
                .Select(entity => entity.Proxy.Proxy)
                .ToList();
            proxies.AddRange(generatedSegments);

            return _modelBuilder.Build(proxies);
        }
    }
}