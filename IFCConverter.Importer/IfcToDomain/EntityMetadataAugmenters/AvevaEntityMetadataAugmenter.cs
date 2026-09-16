using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets;
using IFCConverter.Importer.PropertySets.Aveva;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters
{
    internal sealed class AvevaEntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanAugment(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (context.ImportType != ImportType.AVEVA)
                return false;

            if (!context.TryGetEntityId(product, out EntityId id))
                return false;

            Entity entity = model.GetEntity(id);
            return !(entity is Segment);
        }

        public void Augment(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(product));

            IfcProduct ifcProduct = (IfcProduct)product;
            IPropertySet[] propertySets = ifcProduct.GetPropertySets().ToArray();

            AvevaPset avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset != null)
                entity.Metadata.Meta.Add(nameof(AvevaPset), avevaPset);

            AvevaEntityParameters avevaEntityParameters = propertySets.OfType<AvevaEntityParameters>().FirstOrDefault();
            if (avevaEntityParameters != null)
                entity.Metadata.Meta.Add(nameof(AvevaEntityParameters), avevaEntityParameters);
        }
    }
}