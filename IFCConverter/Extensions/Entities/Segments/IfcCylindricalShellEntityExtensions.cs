using IFC.Entities;
using IFC.Entities.Segments;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Segments
{
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
            
            cylindricalShellEntity.PropertySets.Add(Pset_Start.CreateFromStart(pipeEntity));
            cylindricalShellEntity.PropertySets.Add(Pset_PipeSegmentTypeCommon.CreateFromStart(pipeEntity));
            cylindricalShellEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantities.CreateFromStart(pipeEntity));

            return cylindricalShellEntity;
        }
    }
}