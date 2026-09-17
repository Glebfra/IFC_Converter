using IFCConverter.Domain.Entities;
using IFCConverter.IFC.Builders;
using IFCConverter.IFC.Builders.Relations;
using IFCConverter.IFC.Interfaces;
using IFCConverter.IFC.Interfaces.Relations;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProductExtension;

namespace IFCConverter.Exporter.DomainToIfc.MaterialAugmenters
{
    internal sealed class MaterialAugmenter : IMaterialAugmenter
    {
        public bool CanAugment(Entity entity)
        {
            return !string.IsNullOrEmpty(entity.Metadata.MaterialName);
        }

        public void Augment(Entity entity, IModel model, ExportContext context)
        {
            if (!context.TryGet(entity.Id, out IIfcProduct product))
                return;
            
            string materialName = entity.Metadata.MaterialName;
            
            if (!context.TryGetMaterial(materialName, out IIfcMaterial material))
            {
                IIfcMaterialBuilder materialBuilder = new IfcMaterialBuilder(materialName, $"START material: {materialName}", "START");
                material = materialBuilder.CreateMaterial(model);
                context.RegisterMaterial(materialName, material);
            }

            IIfcRelAssociatesMaterialBuilder<IIfcRelAssociatesMaterial> builder = new IfcRelAssociatesMaterialBuilder<IfcRelAssociatesMaterial>();
            builder.AddMaterial(material);
            builder.AddRelatedObject(product);

            builder.CreateInstance(model);
        }
    }
}