using System;
using IFC.Entities.Abstract;
using Start.API;
using Start.Entities;

namespace IFC.Entities
{
    public static class IfcEntityFactory
    {
        public static IfcAbstractEntity CreateEntity(StartAbstractEntity startAbstractEntity, IfcNodeEntity[] ifcNodeEntities)
        {
            switch (startAbstractEntity.Type)
            {
                case StartElementType.PIPE_ELEMENT:
                    return new IfcPipeEntity((StartPipeEntity)startAbstractEntity, ifcNodeEntities);
                
                case StartElementType.RIGID_ELEMENT:
                    return new IfcRigidElementEntity((StartRigidElementEntity)startAbstractEntity, ifcNodeEntities);
                
                default:
                    throw new Exception("Unknown entity type");
            }
        }
        
        public static IfcAbstractEntity CreateEntity(StartAbstractEntity startAbstractEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] connEntities)
        {
            switch (startAbstractEntity.Type)
            {
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                case StartElementType.SADDLE_BEND:
                    return new IfcBendEntity((StartBendEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.MILTER_JOINT:
                    return new IfcMilterJointEntity((StartBendEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.WELDED_TEE:
                    return new IfcWeldedTeeEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.WELDOLET:
                    return new IfcWeldoletEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.SWEEPOLET:
                    return new IfcSweepoletEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.FABRICATED_TEE:
                    return new IfcFabricatedTeeEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.STUB_IN:
                    return new IfcStubInEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.REDUCER_CONCENTRIC:
                    return new IfcReducerConcentricEntity((StartReducerEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.REDUCER_ECCENTRIC:
                    return new IfcReducerEccentricEntity((StartReducerEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.VALVE:
                    return new IfcValveEntity((StartArmatureEntity)startAbstractEntity, ifcNodeEntity, connEntities);
            
                case StartElementType.FLANGE:
                    return new IfcFlangeEntity((StartArmatureEntity)startAbstractEntity, ifcNodeEntity, connEntities);
                
                default:
                    throw new Exception("Unknown entity type");
            }
        }
    }
}