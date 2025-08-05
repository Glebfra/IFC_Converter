using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcVertexSingleFlangeEntityExtensions
    {
        public static IfcVertexSingleFlangeEntity CreateFromStart(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = armatureEntity.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;

            IfcVertexSingleFlangeEntity singleFlangeEntity = new IfcVertexSingleFlangeEntity(
                armatureEntity.Name,
                armatureEntity.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            singleFlangeEntity.ConnectedEntities.AddRange(segmentEntities);

            return singleFlangeEntity;
        }
    }
    
    #endif
}