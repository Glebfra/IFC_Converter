using System.Linq;
using IFC.PropertySets;
using Start.Entities.Abstract;
using Xbim.Ifc4.MeasureResource;

namespace STARTtoIFC.Extensions.PropertySets
{
    #if NEW
    
    internal static class Pset_PipeSegmentTypeCommonExtensions
    {
        public static Pset_PipeSegmentTypeCommon CreateFromStart(StartAbstractSegmentEntity abstractSegmentEntity)
        {
            Pset_PipeSegmentTypeCommon pset = new Pset_PipeSegmentTypeCommon();
            pset.InnerDiameter = abstractSegmentEntity.InnerDiameter.SIProperty;
            pset.NominalDiameter = abstractSegmentEntity.Diameter.SIProperty;
            pset.OuterDiameter = abstractSegmentEntity.Diameter.SIProperty;
            pset.WorkingPressure = abstractSegmentEntity.Pressure.SIProperty;
            pset.PressureRange = abstractSegmentEntity.PressureRange.Select(item => new IfcPressureMeasure(item.SIProperty)).ToArray();
            pset.TemperatureRange = abstractSegmentEntity.TemperatureRange.Select(item => new IfcThermodynamicTemperatureMeasure(item.SIProperty)).ToArray();

            return pset;
        }
    }
    
    #endif
}