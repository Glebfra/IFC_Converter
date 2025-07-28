using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Segments;
using IFC.Extensions;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Segments
{
    #if NEW
    
    internal static class IfcFlexibleSegmentEntityExtensions
    {
        public static IfcFlexibleSegmentEntity CreateFromStart(StartFlexibleElementEntity flexibleElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(flexibleElement, nodeEntities, out double length);

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