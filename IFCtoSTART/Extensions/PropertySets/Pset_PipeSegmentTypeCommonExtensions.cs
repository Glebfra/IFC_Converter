using IFC.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCtoSTART.Extensions.PropertySets
{
    internal static class Pset_PipeSegmentTypeCommonExtensions
    {
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
                            case nameof(pset.InnerDiameter):
                                pset.InnerDiameter = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                                break;
                            case nameof(pset.NominalDiameter):
                                pset.NominalDiameter = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                                break;
                            case nameof(pset.OuterDiameter):
                                pset.OuterDiameter = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                                break;
                            case nameof(pset.WorkingPressure):
                                pset.WorkingPressure = (IfcPressureMeasure)singleValue.NominalValue;
                                break;
                        }
                        break;
                    case IfcPropertyBoundedValue boundedValue:
                        switch (property.Name)
                        {
                            case nameof(pset.PressureRange):
                                pset.PressureRange[0] = (IfcPressureMeasure)boundedValue.LowerBoundValue;
                                pset.PressureRange[1] = (IfcPressureMeasure)boundedValue.UpperBoundValue;
                                break;
                            case nameof(pset.TemperatureRange):
                                pset.TemperatureRange[0] = (IfcThermodynamicTemperatureMeasure)boundedValue.LowerBoundValue;
                                pset.TemperatureRange[1] = (IfcThermodynamicTemperatureMeasure)boundedValue.UpperBoundValue;
                                break;
                        }
                        break;
                }
            }

            return pset;
        }
    }
}