using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters
{
    internal interface IEntityMetadataAugmenter
    {
        bool CanAugment(IIfcProduct product, EngineeringModel model, ImportContext context);
        void Augment(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}