using IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Registries;

namespace IFCConverter.Exporter.StartToDomain
{
    internal interface IEntityMetadataAugmenterRegistry : IRegistry<IStartEntity, IEntityMetadataAugmenter, StartMappingContext>
    {
    }
}