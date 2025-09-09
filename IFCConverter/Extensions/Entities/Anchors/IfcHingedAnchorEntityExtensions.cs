using IFC.Entities;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
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
            hingedAnchorEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(hingedAnchor));

            return hingedAnchorEntity;
        }
    }
}