using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_PipeFittingTypeJunction : IPropertySet
    {
        public IfcLabel JunctionType;
        public IfcPositiveLengthMeasure JunctionLeftRadius;
        public IfcPositivePlaneAngleMeasure JunctionLeftAngle;
        public IfcPositiveLengthMeasure JunctionRightRadius;
        public IfcPositivePlaneAngleMeasure JunctionRightAngle;

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeJunction";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionType);
                    value.NominalValue = JunctionType;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionLeftRadius);
                    value.NominalValue = JunctionLeftRadius;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionLeftAngle);
                    value.NominalValue = JunctionLeftAngle;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionRightRadius);
                    value.NominalValue = JunctionRightRadius;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionRightAngle);
                    value.NominalValue = JunctionRightAngle;
                }));
            });
        }
    }
}