using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
{
    internal static class IfcGuideDoubleDirectionSupportEntityExtensions
    {
        public static IfcGuideDoubleDirectionSupportEntity CreateFromStart(StartGuideDoubleDirectionSupportEntity doubleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            string name = doubleDirectionSupportEntity.Name;
            string type = doubleDirectionSupportEntity.Type.ToString();
            
            IfcGuideDoubleDirectionSupportEntity guideDoubleDirectionSupportEntity = new IfcGuideDoubleDirectionSupportEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            guideDoubleDirectionSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            guideDoubleDirectionSupportEntity.PropertySets.Add(Pset_Start.CreateFromStart(doubleDirectionSupportEntity));
            
            return guideDoubleDirectionSupportEntity;
        }
    }
}