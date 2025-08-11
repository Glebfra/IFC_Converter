using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Segments;
using IFC.Extensions;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Segments
{
    #if NEW
    
    internal static class IfcRigidElementEntityExtensions
    {
        public static IfcRigidElementEntity CreateFromStart(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreatePipeObjectMatrix(rigidElement, nodeEntities, out double length);
            
            double diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };

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