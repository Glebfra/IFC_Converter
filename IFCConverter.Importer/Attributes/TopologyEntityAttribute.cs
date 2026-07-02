using System;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    internal sealed class TopologyEntityAttribute : Attribute
    {
        public readonly Type? TopologySegmentAugmenterType;

        public TopologyEntityAttribute(Type? topologySegmentAugmenterType = null)
        {
            TopologySegmentAugmenterType = topologySegmentAugmenterType;
        }

        public ITopologySegmentAugmenter? GetTopologySegmentAugmenter()
        {
            if (TopologySegmentAugmenterType == null)
                return null;

            return ParameterlessConstructorRegistry<ITopologySegmentAugmenter>.Create(TopologySegmentAugmenterType);
        }
    }
}