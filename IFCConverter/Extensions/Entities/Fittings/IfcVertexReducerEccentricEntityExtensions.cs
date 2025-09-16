using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.Extensions;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexReducerEccentricEntityExtensions
    {
        public static IfcVertexReducerEccentricEntity CreateFromStart(StartReducerEntity reducer, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            segmentEntities = segmentEntities
                .OrderBy(segment => segment.Diameter.Value)
                .ToArray();
            
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateReducerEccentricObjectMatrix(nodeEntity, segmentEntities, out double displacementLength);

            if ((segmentEntities[1].StartNode.GetDistanceToNode(nodeEntity) < segmentEntities[1].EndNode.GetDistanceToNode(nodeEntity)))
            {
                segmentEntities[1].MovePipe(objectMatrix3D.Up * displacementLength);
            }

            double length = reducer.LengthOfConicalPart.SIProperty;
            double[] diameters = segmentEntities.Select(segment => segment.Diameter.Value).ToArray();

            IfcVertexReducerEccentricEntity reducerEccentricEntity = new IfcVertexReducerEccentricEntity(
                reducer.Name,
                reducer.Type.ToString(),
                objectMatrix3D,
                length,
                displacementLength,
                diameters,
                numSegments
            );

            reducerEccentricEntity.ConnectedEntities.AddRange(segmentEntities);
            reducerEccentricEntity.PropertySets.Add(Pset_Start.CreateFromStart(reducer));
            reducerEccentricEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(reducer));

            return reducerEccentricEntity;
        }
    }
}