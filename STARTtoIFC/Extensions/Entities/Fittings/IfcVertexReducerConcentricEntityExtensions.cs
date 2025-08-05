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
    
    internal static class IfcVertexReducerConcentricEntityExtensions
    {
        public static IfcVertexReducerConcentricEntity CreateFromStart(StartReducerEntity reducer, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateReducerConcentricObjectMatrix(nodeEntity, segmentEntities);

            double length = reducer.LengthOfConicalPart.SIProperty;
            double[] diameters = segmentEntities.Select(segment => segment.Diameter.Value).ToArray();

            IfcVertexReducerConcentricEntity reducerConcentricEntity = new IfcVertexReducerConcentricEntity(
                reducer.Name,
                reducer.Type.ToString(),
                objectMatrix3D,
                length,
                diameters,
                numSegments
            );
            
            reducerConcentricEntity.ConnectedEntities.AddRange(segmentEntities);

            return reducerConcentricEntity;
        }
    }
    
    #endif
}