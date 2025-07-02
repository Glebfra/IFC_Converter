using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.EntitiesExtensions
{
    public class Pset_PipeFittingTypeJunction : IPropertySet
    {
        public IfcLabel JunctionType;
        public IfcPositiveLengthMeasure JunctionLeftRadius;
        public IfcPositivePlaneAngleMeasure JunctionLeftAngle;
        public IfcPositiveLengthMeasure JunctionRightRadius;
        public IfcPositivePlaneAngleMeasure JunctionRightAngle;
        
        public Pset_PipeFittingTypeJunction() {}

        public static Pset_PipeFittingTypeJunction CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Pset_PipeFittingTypeJunction pset = new Pset_PipeFittingTypeJunction();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(JunctionType):
                        pset.JunctionType = (IfcLabel)singleValue.NominalValue;
                        break;
                    case nameof(JunctionLeftRadius):
                        pset.JunctionLeftRadius = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                        break;
                    case nameof(JunctionLeftAngle):
                        pset.JunctionLeftAngle = (IfcPositivePlaneAngleMeasure)singleValue.NominalValue;
                        break;
                    case nameof(JunctionRightRadius):
                        pset.JunctionRightRadius = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                        break;
                    case nameof(JunctionRightAngle):
                        pset.JunctionRightAngle = (IfcPositivePlaneAngleMeasure)singleValue.NominalValue;
                        break;
                }
            }

            return pset;
        }

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