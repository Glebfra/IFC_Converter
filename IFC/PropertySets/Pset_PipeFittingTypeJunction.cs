using IFC.Tools;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_PipeFittingTypeJunction : IPropertySet
    {
        public ActionProperty<IfcLabel> JunctionType = new ActionProperty<IfcLabel>("TEE");
        public ActionProperty<IfcPositiveLengthMeasure> JunctionLeftRadius = new ActionProperty<IfcPositiveLengthMeasure>(0.0);
        public ActionProperty<IfcPositivePlaneAngleMeasure> JunctionLeftAngle = new ActionProperty<IfcPositivePlaneAngleMeasure>(0.0);
        public ActionProperty<IfcPositiveLengthMeasure> JunctionRightRadius = new ActionProperty<IfcPositiveLengthMeasure>(0.0);
        public ActionProperty<IfcPositivePlaneAngleMeasure> JunctionRightAngle = new ActionProperty<IfcPositivePlaneAngleMeasure>(0.0);

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeJunction";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionType);
                    value.NominalValue = JunctionType.Value;
                    
                    JunctionType.OnValueChange += () => value.NominalValue = JunctionType.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionLeftRadius);
                    value.NominalValue = JunctionLeftRadius.Value;
                    
                    JunctionLeftRadius.OnValueChange += () => value.NominalValue = JunctionLeftRadius.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionLeftAngle);
                    value.NominalValue = JunctionLeftAngle.Value;
                    
                    JunctionLeftAngle.OnValueChange += () => value.NominalValue = JunctionLeftAngle.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionRightRadius);
                    value.NominalValue = JunctionRightRadius.Value;
                    
                    JunctionRightRadius.OnValueChange += () => value.NominalValue = JunctionRightRadius.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(JunctionRightAngle);
                    value.NominalValue = JunctionRightAngle.Value;
                    
                    JunctionRightAngle.OnValueChange += () => value.NominalValue = JunctionRightAngle.Value;
                }));
            });
        }
    }
}