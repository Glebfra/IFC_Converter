using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
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
        
        public static Pset_PipeFittingTypeBend CreateFromStart(StartBendEntity bendEntity)
        {
            return new Pset_PipeFittingTypeBend()
            {
                BendRadius = new ActionProperty<IfcPositiveLengthMeasure>(bendEntity.Radius.SIProperty),
            };
        }
        
        public static Pset_PipeFittingTypeBend CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Pset_PipeFittingTypeBend pset = new Pset_PipeFittingTypeBend();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(pset.BendAngle):
                        pset.BendAngle = (IfcPositivePlaneAngleMeasure)singleValue.NominalValue;
                        break;
                    case nameof(pset.BendRadius):
                        pset.BendRadius = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                        break;
                }
            }

            return pset;
        }
    }
}