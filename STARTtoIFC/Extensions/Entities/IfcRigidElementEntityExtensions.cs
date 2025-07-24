using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Segments;
using IFC.Extensions;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.PropertySets;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities
{
    #if NEW
    
    internal static class IfcRigidElementEntityExtensions
    {
        public static IfcRigidElementEntity CreateFromStart(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimVector3D coordinates = nodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D direction = nodeEntities[1].ObjectMatrix3D.Translation - coordinates;
            
            double diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };

            XbimVector3D forward = direction.Normalized();
            XbimMatrix3D objectMatrix3D = MatrixExtensions.CreateWorld(coordinates, forward);
            double length = direction.Length;

            IfcRigidElementEntity ifcRigidElementEntity = new IfcRigidElementEntity(
                rigidElement.Name,
                rigidElement.Type.ToString(),
                objectMatrix3D,
                length,
                diameter
            );
            ifcRigidElementEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(rigidElement));
            ifcRigidElementEntity.PropertySets.Add(Pset_PipeSegmentTypeCommonExtensions.CreateFromStart(rigidElement));
            ifcRigidElementEntity.PropertySets.Add(Qto_PipeSegmentBaseQuantitiesExtensions.CreateFromStart(rigidElement));

            return ifcRigidElementEntity;
        }
    }
    
    #endif
}