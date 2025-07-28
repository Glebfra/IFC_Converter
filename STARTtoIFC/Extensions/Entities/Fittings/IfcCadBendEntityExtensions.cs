using System;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcCadBendEntityExtensions
    {
        public static IfcCadBendEntity CreateFromStart(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double bendRadius = bendEntity.Radius.SIProperty;
            double pipeRadius = Math.Min(segmentEntities[0].Diameter / 2, segmentEntities[1].Diameter / 2);
            
            double length = angle * bendRadius;

            IfcCadBendEntity cadBendEntity = new IfcCadBendEntity(
                bendEntity.Name,
                bendEntity.Type.ToString(),
                objectMatrix3D,
                length, 
                angle,
                bendRadius,
                pipeRadius
            );
            
            cadBendEntity.ConnectedEntities.AddRange(segmentEntities);
            cadBendEntity.PropertySets.Add(Pset_PipeFittingTypeBendExtensions.CreateFromStart(bendEntity));

            return cadBendEntity;
        }
    }
    
    #endif
}