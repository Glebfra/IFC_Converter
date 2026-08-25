using IFCConverter.Domain;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal interface IEntityMetadataAugmenter
    {
        bool CanResolve(IStartEntity source, StartMappingContext context);
        void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context);
    }
}