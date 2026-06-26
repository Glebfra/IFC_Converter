using System;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Attributes
{
    internal sealed class TopologyEntityAttribute : Attribute
    {
        public TopologyEntityAttribute(Type connectionAugmenterType)
        {
            ConnectionAugmenterType = connectionAugmenterType;
        }

        public Type ConnectionAugmenterType { get; }

        public IEntityConnectionAugmenter GetConnectionAugmenter()
        {
            return (IEntityConnectionAugmenter)Activator.CreateInstance(ConnectionAugmenterType);
        }
    }
}