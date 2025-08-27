using IFC.Tools;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_PipeSegmentTypeCommon : IPropertySet
    {
        public ActionProperty<IfcPositiveLengthMeasure> InnerDiameter = new ActionProperty<IfcPositiveLengthMeasure>(0.0);
        public ActionProperty<IfcPositiveLengthMeasure> NominalDiameter = new ActionProperty<IfcPositiveLengthMeasure>(0.0);
        public ActionProperty<IfcPositiveLengthMeasure> OuterDiameter = new ActionProperty<IfcPositiveLengthMeasure>(0.0);
        public ActionProperty<IfcPressureMeasure> WorkingPressure = new ActionProperty<IfcPressureMeasure>(0.0);
        public IfcPressureMeasure[] PressureRange = new IfcPressureMeasure[2];
        public IfcThermodynamicTemperatureMeasure[] TemperatureRange = new IfcThermodynamicTemperatureMeasure[2];

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = nameof(Pset_PipeSegmentTypeCommon);
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(OuterDiameter);
                    value.NominalValue = OuterDiameter.Value;
                    
                    OuterDiameter.OnValueChange += () => value.NominalValue = OuterDiameter.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(InnerDiameter);
                    value.NominalValue = InnerDiameter.Value;
                    
                    InnerDiameter.OnValueChange += () => value.NominalValue = InnerDiameter.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(NominalDiameter);
                    value.NominalValue = NominalDiameter.Value;
                    
                    NominalDiameter.OnValueChange += () => value.NominalValue = NominalDiameter.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(WorkingPressure);
                    value.NominalValue = WorkingPressure.Value;
                    
                    WorkingPressure.OnValueChange += () => value.NominalValue = WorkingPressure.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertyBoundedValue>(value =>
                {
                    value.Name = nameof(PressureRange);
                    value.LowerBoundValue = PressureRange[0];
                    value.UpperBoundValue = PressureRange[1];
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertyBoundedValue>(value =>
                {
                    value.Name = nameof(TemperatureRange);
                    value.LowerBoundValue = TemperatureRange[0];
                    value.UpperBoundValue = TemperatureRange[1];
                }));
            });
        }
    }
}