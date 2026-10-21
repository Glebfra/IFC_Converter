using IFCConverter.Domain.Metadata;

namespace IFCConverter.Domain.Entities
{
    public interface IEntityTypedMetadata<out TMetadata>
        where TMetadata : IEntityMetadata
    {
        TMetadata TypedMetadata { get; }
    }
}