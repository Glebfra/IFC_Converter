using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ifc.Builders.Properties;
using Ifc.Builders.Relations;
using Ifc.Interfaces;
using Ifc.Interfaces.Relations;
using IFCConverter.Domain.Entities;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCConverter.Exporter.DomainToIfc.PropertySetAugmenters
{
    internal class PipeSegmentPropertySetAugmenter : IPropertySetAugmenter
    {
        private const string PsetName = "Pset_PipeSegmentTypeCommon";
        
        public bool CanAugment(Entity entity, ExportContext context)
        {
            if (!context.TryGet(entity.Id, out IIfcProduct product))
                return false;
            
            return product is IIfcPipeSegment;
        }

        public void Augment(Entity entity, IModel model, ExportContext context)
        {
            IIfcProduct product = context.Get(entity.Id);
            List<IIfcPropertyBuilder<IIfcProperty>> propertyBuilders = new List<IIfcPropertyBuilder<IIfcProperty>>();
            
            Regex regex = new Regex(@"[+-]?\d+(?:[.,]\d+)?");
            if (entity.Metadata.Properties.TryGetValue("Pressure", out object? pressureValue))
            {
                Match match = regex.Match(pressureValue!.ToString());
                propertyBuilders.Add(new IfcPropertySingleValueBuilder<IfcPropertySingleValue>(
                    "WorkingPressure", "",
                    new IfcPressureMeasure(Convert.ToDouble(match.Value)), null!));
            }
            
            if (entity.Metadata.Properties.TryGetValue("Diameter", out object? outerDiameterValue))
            {
                Match match = regex.Match(outerDiameterValue!.ToString());
                double outerDiameter = Convert.ToDouble(match.Value);
                propertyBuilders.Add(new IfcPropertySingleValueBuilder<IfcPropertySingleValue>(
                    "OuterDiameter", "", 
                    new IfcPositiveLengthMeasure(outerDiameter), null!));

                if (entity.Metadata.Properties.TryGetValue("WallThickness", out object? wallThicknessValue))
                {
                    Match matchThickness = regex.Match(wallThicknessValue!.ToString());
                    double wallThickness = Convert.ToDouble(matchThickness.Value);
                    propertyBuilders.Add(new IfcPropertySingleValueBuilder<IfcPropertySingleValue>(
                        "InnerDiameter", "",
                        new IfcPositiveLengthMeasure(outerDiameter - wallThickness), null!));
                }
            }
            
            IIfcPropertySetBuilder propertySetBuilder = new IfcPropertySetBuilder(PsetName, propertyBuilders);
            IIfcPropertySet propertySet = propertySetBuilder.CreatePropertySet(model);
            
            IIfcRelDefinesByPropertiesBuilder<IIfcRelDefinesByProperties> relDefinesByPropertiesBuilder =
                new IfcRelDefinesByPropertiesBuilder<IfcRelDefinesByProperties>();
            relDefinesByPropertiesBuilder.AddPropertySet(propertySet);
            relDefinesByPropertiesBuilder.AddRelatedObject(product);

            relDefinesByPropertiesBuilder.CreateInstance(model);
        }
    }
}