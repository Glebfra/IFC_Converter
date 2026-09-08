using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using IFCConverter.Utils.Registries;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IEntityMetadataAugmenterRegistry : IRegistry<IStartEntity, IEntityMetadataAugmenter, StartMappingContext>
    {
    }
}