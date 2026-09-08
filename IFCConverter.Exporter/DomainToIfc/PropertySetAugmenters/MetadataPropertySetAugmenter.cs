using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.Builders.Properties;
using IFCConverter.IFC.Builders.Relations;
using IFCConverter.IFC.Interfaces.Properties;
using IFCConverter.IFC.Interfaces.Relations;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters
{
    internal sealed class MetadataPropertySetAugmenter : IPropertySetAugmenter
    {
        private const string PsetName = "Pset_Start";

        public bool CanAugment(Entity entity, ExportContext context)
        {
            return context.TryGet(entity.Id, out _);
        }

        public void Augment(Entity entity, IModel model, ExportContext context)
        {
            IIfcProduct product = context.Get(entity.Id);
            IEnumerable<IIfcPropertyBuilder<IIfcPropertySingleValue>> propertyBuilders = entity.Metadata.Properties
                .Select(pair =>
                {
                    string propertyName = pair.Key;
                    IfcText propertyValue = new IfcText(pair.Value?.ToString());
                    string propertyDescription = "";
                    return new IfcPropertySingleValueBuilder<IfcPropertySingleValue>(propertyName, propertyDescription, propertyValue, null);
                });

            IIfcPropertySetBuilder propertySetBuilder = new IfcPropertySetBuilder(PsetName, propertyBuilders);
            IIfcPropertySet propertySet = propertySetBuilder.CreatePropertySet(model);

            IIfcRelDefinesByPropertiesBuilder<IfcRelDefinesByProperties> relDefinesByPropertiesBuilder =
                new IfcRelDefinesByPropertiesBuilder<IfcRelDefinesByProperties>();
            relDefinesByPropertiesBuilder.AddRelatedObject(product);
            relDefinesByPropertiesBuilder.AddPropertySet(propertySet);

            relDefinesByPropertiesBuilder.CreateInstance(model);
        }
    }
}