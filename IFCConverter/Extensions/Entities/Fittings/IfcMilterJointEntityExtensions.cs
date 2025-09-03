using System;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcMilterJointEntityExtensions
    {
        public static IfcMilterJointEntity CreateFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
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
}