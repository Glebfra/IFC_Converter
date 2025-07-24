using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Segments;
using IFC.Extensions;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities
{
    #if NEW
    
    internal static class IfcFlexibleSegmentEntityExtensions
    {
        public static IfcFlexibleSegmentEntity CreateFromStart(StartFlexibleElementEntity flexibleElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D direction = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            double length = direction.Length;

            XbimVector3D forward = direction.Normalized();
            XbimMatrix3D objectMatrix3D = MatrixExtensions.CreateWorld(coordinates, forward);

            double diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };

            return new IfcFlexibleSegmentEntity(
                flexibleElement.Name,
                flexibleElement.Type.ToString(),
                objectMatrix3D,
                length,
                diameter
            );
        }
    }
    
    #endif
}