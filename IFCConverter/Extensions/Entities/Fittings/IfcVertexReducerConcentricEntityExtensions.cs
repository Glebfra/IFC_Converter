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
    internal static class IfcVertexReducerConcentricEntityExtensions
    {
        public static IfcVertexReducerConcentricEntity CreateFromStart(StartReducerEntity reducer, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            segmentEntities = segmentEntities.OrderBy(segment => segment.Diameter.Value).ToArray();
            
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
            reducerConcentricEntity.PropertySets.Add(Pset_Start.CreateFromStart(reducer));
            reducerConcentricEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(reducer));

            return reducerConcentricEntity;
        }
    }
}