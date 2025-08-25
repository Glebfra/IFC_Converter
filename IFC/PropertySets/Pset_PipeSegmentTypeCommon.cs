using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class Pset_PipeSegmentTypeCommon : IPropertySet
    {
        public IfcPositiveLengthMeasure InnerDiameter;
        public IfcPositiveLengthMeasure NominalDiameter;
        public IfcPositiveLengthMeasure OuterDiameter;
        public IfcPressureMeasure WorkingPressure;
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
                    value.NominalValue = OuterDiameter;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(InnerDiameter);
                    value.NominalValue = InnerDiameter;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(NominalDiameter);
                    value.NominalValue = NominalDiameter;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(WorkingPressure);
                    value.NominalValue = WorkingPressure;
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