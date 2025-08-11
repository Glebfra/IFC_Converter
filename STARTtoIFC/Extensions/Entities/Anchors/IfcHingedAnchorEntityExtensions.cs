using IFC.Entities;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal static class IfcHingedAnchorEntityExtensions
    {
        public static IfcAbstractHingedAnchorEntity CreateFromStart(StartHingedAnchorEntity hingedAnchor, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;

            IfcAbstractHingedAnchorEntity hingedAnchorEntity = new IfcHingedAnchorEntity(
                hingedAnchor.Name,
                hingedAnchor.Type.ToString(),
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            hingedAnchorEntity.ConnectedEntities.AddRange(segmentEntities);

            return hingedAnchorEntity;
        }
    }
}