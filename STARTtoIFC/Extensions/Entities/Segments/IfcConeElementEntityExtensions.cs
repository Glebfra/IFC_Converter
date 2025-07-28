using IFC.Entities;
using IFC.Entities.Segments;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Segments
{
    #if NEW
    
    internal static class IfcConeElementEntityExtensions
    {
        public static IfcConeElementEntity CreateFromStart(StartConeElementEntity coneElement, IfcNodeEntity[] nodeEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(coneElement, nodeEntities, out double length);

            double diameter = coneElement.Diameter.SIProperty;
            double secondDiameter = coneElement.SecondDiameter.SIProperty;

            return new IfcConeElementEntity(
                coneElement.Name,
                coneElement.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                secondDiameter,
                numSegments
            );
        }
    }
    
    #endif
}