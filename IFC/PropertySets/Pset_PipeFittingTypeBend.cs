using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_PipeFittingTypeBend : IPropertySet
    {
        public IfcPositivePlaneAngleMeasure BendAngle;
        public IfcPositiveLengthMeasure BendRadius;

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