using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
{
    internal static class IfcNonStandardRestraintEntityExtensions
    {
        public static IfcNonStandardRestraintEntity CreateFromStart(StartNonStandardRestraint nonStandardRestraint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            IfcNonStandardRestraintEntity restraintEntity = new IfcNonStandardRestraintEntity(
                nonStandardRestraint.Name,
                nonStandardRestraint.Type.ToString(),
                objectMatrix3D,
                nonStandardRestraint,
                diameter,
                height,
                numSegments
            );
            
            restraintEntity.ConnectedEntities.AddRange(segmentEntities);
            restraintEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(nonStandardRestraint));
            
            return restraintEntity;
        }
    }
}