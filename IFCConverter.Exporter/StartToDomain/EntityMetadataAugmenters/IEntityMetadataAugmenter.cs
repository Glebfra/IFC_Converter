using IFCConverter.Domain;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal interface IEntityMetadataAugmenter
    {
        bool CanResolve(IStartEntity source, EngineeringModel model, StartMappingContext context);
        void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context);
    }
}