using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IEntityMetadataAugmenterRegistry
    {
        IEntityMetadataAugmenter Resolve(IStartEntity entity, EngineeringModel model, StartMappingContext context);
        IEnumerable<IEntityMetadataAugmenter> ResolveAll(IStartEntity source, EngineeringModel model, StartMappingContext context);
        bool TryResolve(IStartEntity entity, EngineeringModel model, StartMappingContext context, out IEntityMetadataAugmenter registration);
    }
}