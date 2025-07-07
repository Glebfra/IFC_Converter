using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_PipeFittingTypeBend : IPropertySet
    {
        public IfcPositivePlaneAngleMeasure BendAngle;
        public IfcPositiveLengthMeasure BendRadius;
        
        public Pset_PipeFittingTypeBend() {}

        public static Pset_PipeFittingTypeBend CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Pset_PipeFittingTypeBend pset = new Pset_PipeFittingTypeBend();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(BendAngle):
                        pset.BendAngle = (IfcPositivePlaneAngleMeasure)singleValue.NominalValue;
                        break;
                    case nameof(BendRadius):
                        pset.BendRadius = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                        break;
                }
            }

            return pset;
        }
        
        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeBend";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(BendAngle);
                    value.NominalValue = BendAngle;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(BendRadius);
                    value.NominalValue = BendRadius;
                }));
            });
        }
    }
}