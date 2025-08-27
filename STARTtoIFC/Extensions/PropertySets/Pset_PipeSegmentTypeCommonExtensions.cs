using System.Linq;
using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Ifc4.MeasureResource;

namespace STARTtoIFC.Extensions.PropertySets
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
    }
}