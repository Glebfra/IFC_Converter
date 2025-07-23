using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Anchors;

namespace IFC.Entities.Anchors.CAD
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcRigidHangerEntity : IfcAbstractRigidHangerEntity
    {
        public override double Diameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Height { get; protected set; }
        
        public IfcRigidHangerEntity(StartRigidHangerEntity rigidHangerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(rigidHangerEntity, nodeEntity, segmentEntities)
        {
            NumSegments = 8;
            Diameter = AbstractSegmentEntities[0].Diameter;
            Height = Diameter * 2;
        }
    }

    #endif
}