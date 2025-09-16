using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
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
            flangeEntity.PropertySets.Add(Pset_Start.CreateFromStart(armature));
            flangeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(armature));

            return flangeEntity;
        }
    }
}