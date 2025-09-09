using IFC.Entities;
using IFC.Entities.Segments;
using IFCConverter.Extensions.PropertySets;
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
            
            coneElementEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(coneElement));
            coneElementEntity.PropertySets.Add(Pset_PipeSegmentTypeCommonExtensions.CreateFromStart(coneElement));
            coneElementEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantitiesExtensions.CreateFromStart(coneElement));

            return coneElementEntity;
        }
    }
}