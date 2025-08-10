using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Tools;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcMilterJointEntityExtensions
    {
        public static IfcMilterJointEntity CreateFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = 2 * Math.Min(segmentEntities[0].Length.Value, segmentEntities[1].Length.Value) * 0.1;

            IfcMilterJointEntity milterJointEntity = new IfcMilterJointEntity(
                bendEntity.Name,
                bendEntity.Type.ToString(),
                objectMatrix3D,
                length,
                diameter
            );
            
            milterJointEntity.ConnectedEntities.AddRange(segmentEntities);
            milterJointEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(bendEntity));
            milterJointEntity.PropertySets.Add(Pset_PipeFittingTypeBendExtensions.CreateFromStart(bendEntity));
            milterJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(bendEntity));

            return milterJointEntity;
        }
    }
    
    #endif
}