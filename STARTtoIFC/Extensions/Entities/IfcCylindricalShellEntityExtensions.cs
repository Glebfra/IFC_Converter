using IFC.Entities;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.PropertySets;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities
{
    #if NEW
    
    internal static class IfcCylindricalShellEntityExtensions
    {
        public static IfcCylindricalShellEntity CreateFromStart(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D direction = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            double length = direction.Length;

            XbimVector3D forward = direction.Normalized();
            XbimMatrix3D objectMatrix3D = MatrixExtensions.CreateWorld(coordinates, forward);

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