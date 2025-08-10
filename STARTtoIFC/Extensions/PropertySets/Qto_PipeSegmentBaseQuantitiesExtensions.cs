using IFC.PropertySets;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.PropertySets
{
    #if NEW
    
    internal static class Qto_PipeSegmentBaseQuantitiesExtensions
    {
        public static Qto_PipeSegmentBaseQuantities CreateFromStart(StartAbstractSegmentEntity abstractSegmentEntity)
        {
            Qto_PipeSegmentBaseQuantities qto = new Qto_PipeSegmentBaseQuantities();
            qto.NetWeight = abstractSegmentEntity.PipeUnitWeight.SIProperty;
            
            XbimVector3D projection = new XbimVector3D(
                abstractSegmentEntity.ProjectionAlongOXAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOYAxis.SIProperty,
                abstractSegmentEntity.ProjectionAlongOZAxis.SIProperty
            );
            qto.Length = projection.Length;
            return qto;
        }
    }
    
    #endif
}