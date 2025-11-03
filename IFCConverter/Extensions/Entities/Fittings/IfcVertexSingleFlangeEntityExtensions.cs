using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexSingleFlangeEntityExtensions
    {
        public static IfcVertexSingleFlangeEntity CreateFromStart(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = armatureEntity.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;
            
            string name = armatureEntity.Name;
            string type = armatureEntity.Type.ToString();

            IfcVertexSingleFlangeEntity singleFlangeEntity = new IfcVertexSingleFlangeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            singleFlangeEntity.ConnectedEntities.AddRange(segmentEntities);
            singleFlangeEntity.PropertySets.Add(Pset_Start.CreateFromStart(armatureEntity));
            singleFlangeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(armatureEntity));

            return singleFlangeEntity;
        }
    }
}