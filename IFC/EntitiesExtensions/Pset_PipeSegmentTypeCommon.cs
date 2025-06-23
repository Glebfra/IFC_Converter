using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.EntitiesExtensions
{
    public class Pset_PipeSegmentTypeCommon : IPropertySet
    {
        public IfcPositiveLengthMeasure InnerDiameter;
        public IfcPositiveLengthMeasure NominalDiameter;
        public IfcPositiveLengthMeasure OuterDiameter;
        public IfcPressureMeasure WorkingPressure;
        public IfcPressureMeasure[] PressureRange = new IfcPressureMeasure[2];
        public IfcThermodynamicTemperatureMeasure[] TemperatureRange = new IfcThermodynamicTemperatureMeasure[2];
        
        public Pset_PipeSegmentTypeCommon() {}

        public static Pset_PipeSegmentTypeCommon CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Pset_PipeSegmentTypeCommon pset = new Pset_PipeSegmentTypeCommon();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                switch (property)
                {
                    case IfcPropertySingleValue singleValue:
                        switch (property.Name)
                        {
                            case nameof(InnerDiameter):
                                pset.InnerDiameter = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                                break;
                            case nameof(NominalDiameter):
                                pset.NominalDiameter = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                                break;
                            case nameof(OuterDiameter):
                                pset.OuterDiameter = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                                break;
                            case nameof(WorkingPressure):
                                pset.WorkingPressure = (IfcPressureMeasure)singleValue.NominalValue;
                                break;
                        }
                        break;
                    case IfcPropertyBoundedValue boundedValue:
                        switch (property.Name)
                        {
                            case nameof(PressureRange):
                                pset.PressureRange[0] = (IfcPressureMeasure)boundedValue.LowerBoundValue;
                                pset.PressureRange[1] = (IfcPressureMeasure)boundedValue.UpperBoundValue;
                                break;
                            case nameof(TemperatureRange):
                                pset.TemperatureRange[0] = (IfcThermodynamicTemperatureMeasure)boundedValue.LowerBoundValue;
                                pset.TemperatureRange[1] = (IfcThermodynamicTemperatureMeasure)boundedValue.UpperBoundValue;
                                break;
                        }
                        break;
                }
            }

            return pset;
        }

        public IfcPropertySet CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeSegmentTypeCommon";
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