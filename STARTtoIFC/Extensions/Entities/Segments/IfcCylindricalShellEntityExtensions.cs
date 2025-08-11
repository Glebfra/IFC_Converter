using IFC.Entities;
using IFC.Entities.Segments;
using IFC.Extensions;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Segments
{
    #if NEW
    
    internal static class IfcCylindricalShellEntityExtensions
    {
        public static IfcCylindricalShellEntity CreateFromStart(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(pipeEntity, nodeEntities, out double length);
            double diameter = pipeEntity.Diameter.SIProperty;

            IfcCylindricalShellEntity cylindricalShellEntity = new IfcCylindricalShellEntity(
                pipeEntity.Name,
                pipeEntity.Type.ToString(),
                objectMatrix3D,
                length,
                diameter
            );
            
            cylindricalShellEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(pipeEntity));
            cylindricalShellEntity.PropertySets.Add(Pset_PipeSegmentTypeCommonExtensions.CreateFromStart(pipeEntity));
            cylindricalShellEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantitiesExtensions.CreateFromStart(pipeEntity));

            return cylindricalShellEntity;
        }
    }
    
    #endif
}