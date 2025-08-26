using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace STARTtoIFC.Extensions.PropertySets
{
    internal static class Qto_PipeSegmentBaseQuantitiesExtensions
    {
        public static Qto_PipeSegmentBaseQuantities CreateFromStart(StartAbstractSegmentEntity abstractSegmentEntity)
        {
            Qto_PipeSegmentBaseQuantities qto = new Qto_PipeSegmentBaseQuantities();
            qto.NetWeight = new ActionProperty<IfcMassMeasure>(abstractSegmentEntity.PipeUnitWeight.SIProperty);
            
            XbimVector3D projection = new XbimVector3D(
                abstractSegmentEntity.ProjectionAlongOXAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOYAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOZAxis.SIProperty
            );
            qto.Length = new ActionProperty<IfcLengthMeasure>(projection.Length);
            return qto;
        }
    }
}