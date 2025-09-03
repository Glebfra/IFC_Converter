using System.Linq;
using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCConverter.Extensions.PropertySets
{
    internal static class Pset_PipeSegmentTypeCommonExtensions
    {
        public static Pset_PipeSegmentTypeCommon CreateFromStart(StartAbstractSegmentEntity abstractSegmentEntity)
        {
            Pset_PipeSegmentTypeCommon pset = new Pset_PipeSegmentTypeCommon();
            pset.InnerDiameter = new ActionProperty<IfcPositiveLengthMeasure>(abstractSegmentEntity.InnerDiameter.SIProperty);
            pset.NominalDiameter = new ActionProperty<IfcPositiveLengthMeasure>(abstractSegmentEntity.Diameter.SIProperty);
            pset.OuterDiameter = new ActionProperty<IfcPositiveLengthMeasure>(abstractSegmentEntity.Diameter.SIProperty);
            pset.WorkingPressure = new ActionProperty<IfcPressureMeasure>(abstractSegmentEntity.Pressure.SIProperty);
            pset.PressureRange = abstractSegmentEntity.PressureRange.Select(item => new IfcPressureMeasure(item.SIProperty)).ToArray();
            pset.TemperatureRange = abstractSegmentEntity.TemperatureRange.Select(item => new IfcThermodynamicTemperatureMeasure(item.SIProperty)).ToArray();

            return pset;
        }
        
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