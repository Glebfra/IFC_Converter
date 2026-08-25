using System.Collections.Generic;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IEntityMetadataAugmenterRegistry
    {
        public IEnumerable<IEntityMetadataAugmenter> ResolveAll(IStartEntity source, StartMappingContext context);
    }
}