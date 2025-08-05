using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcVertexFlangeEntityExtensions
    {
        public static IfcVertexFlangeEntity CreateFromStart(StartArmatureEntity armature, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = armature.Length.SIProperty;
            double[] diameters = segmentEntities.Select(segment => segment.Diameter.Value).ToArray();

            IfcVertexFlangeEntity flangeEntity = new IfcVertexFlangeEntity(
                armature.Name,
                armature.Type.ToString(),
                objectMatrix3D,
                length,
                diameters,
                numSegments
            );
            
            flangeEntity.ConnectedEntities.AddRange(segmentEntities);

            return flangeEntity;
        }
    }
    
    #endif
}