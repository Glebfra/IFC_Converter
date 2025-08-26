using IFC.Tools;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_PipeFittingTypeBend : IPropertySet
    {
        public ActionProperty<IfcPositivePlaneAngleMeasure> BendAngle = new ActionProperty<IfcPositivePlaneAngleMeasure>(0.0);
        public ActionProperty<IfcPositiveLengthMeasure> BendRadius = new ActionProperty<IfcPositiveLengthMeasure>(0.0);

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeBend";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(BendAngle);
                    value.NominalValue = BendAngle.Value;

                    BendAngle.OnValueChange += () => value.NominalValue = BendAngle.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(BendRadius);
                    value.NominalValue = BendRadius.Value;
                    
                    BendRadius.OnValueChange += () => value.NominalValue = BendRadius.Value;
                }));
            });
        }
    }
}