using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Normalizers;
using IFCConverter.Importer.Topology;
using Utils;

namespace IFCConverter.Importer.TopologyAugmenter
{
    internal sealed class NormalizeTopologyAugmenter : ITopologyAugmenter
    {
        private readonly TopologyModelBuilder _modelBuilder;
        private readonly ISegmentNormalizer _segmentNormalizer = SegmentNormalizer.GetInstance();

        public NormalizeTopologyAugmenter(VectorComparer comparer)
        {
            _modelBuilder = new TopologyModelBuilder(comparer);
        }

        public ITopologyModel Augment(ITopologyModel model)
        {
            IReadOnlyCollection<ISegmentProxy> segments = model.Entities
                .Select(entity => entity.Proxy.Proxy)
                .OfType<ISegmentProxy>()
                .ToArray();

            IReadOnlyCollection<ISegmentProxy> normalizedSegments = _segmentNormalizer.Normalize(segments);

            List<IEntityProxy> proxies = model.Entities
                .Select(entity => entity.Proxy.Proxy)
                .OfType<IFittingProxy>()
                .Cast<IEntityProxy>()
                .ToList();

            proxies.AddRange(normalizedSegments);

            return _modelBuilder.Build(proxies);
        }
    }
}