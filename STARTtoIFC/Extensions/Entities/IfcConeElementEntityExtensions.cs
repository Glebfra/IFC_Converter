using IFC.Entities;
using IFC.Entities.Segments;
using IFC.Extensions;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities
{
    #if NEW
    
    internal static class IfcConeElementEntityExtensions
    {
        public static IfcConeElementEntity CreateFromStart(StartConeElementEntity coneElement, IfcNodeEntity[] nodeEntities, int numSegments)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D nodesDirection = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            XbimVector3D pipeProjection = new XbimVector3D(
                coneElement.ProjectionAlongOXAxis.SIProperty,
                coneElement.ProjectionAlongOYAxis.SIProperty,
                coneElement.ProjectionAlongOZAxis.SIProperty
            );
            double length = pipeProjection.Length;
            XbimVector3D direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            
            XbimVector3D forward = direction.Normalized();
            XbimMatrix3D ObjectMatrix3D = MatrixExtensions.CreateWorld(coordinates, forward);

            double diameter = coneElement.Diameter.SIProperty;
            double secondDiameter = coneElement.SecondDiameter.SIProperty;

            return new IfcConeElementEntity(
                coneElement.Name,
                coneElement.Type.ToString(),
                ObjectMatrix3D,
                length,
                diameter,
                secondDiameter,
                numSegments
            );
        }
    }
    
    #endif
}