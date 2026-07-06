using System;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    internal sealed class TopologyEntityAttribute : Attribute
    {
        public readonly Type? SegmentAugmenterType;

        public TopologyEntityAttribute(Type? segmentAugmenterType = null)
        {
            SegmentAugmenterType = segmentAugmenterType;
        }

        [Pure]
        public ITopologySegmentAugmenter? GetSegmentAugmenter()
        {
            if (SegmentAugmenterType == null)
                return null;

            return ParameterlessConstructorRegistry<ITopologySegmentAugmenter>.Create(SegmentAugmenterType);
        }
    }
}