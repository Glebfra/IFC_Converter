using IFC.Entities;
using IFC.Entities.Segments;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Segments
{
    internal static class IfcConeElementEntityExtensions
    {
        public static IfcConeElementEntity CreateFromStart(StartConeElementEntity coneElement, IfcNodeEntity[] nodeEntities)
        {
            int numSegments = 16;
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(coneElement, nodeEntities, out double length);

            double diameter = coneElement.Diameter.SIProperty;
            double secondDiameter = coneElement.SecondDiameter.SIProperty;

            IfcConeElementEntity coneElementEntity = new IfcConeElementEntity(
                coneElement.Name,
                coneElement.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                secondDiameter,
                numSegments
            );
            
            coneElementEntity.PropertySets.Add(Pset_Start.CreateFromStart(coneElement));
            coneElementEntity.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(coneElement));
            coneElementEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(coneElement));

            return coneElementEntity;
        }
    }
}