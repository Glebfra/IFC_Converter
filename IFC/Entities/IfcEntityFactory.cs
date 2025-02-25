using System;
using IFC.Entities.Abstract;
using Start.API;
using Start.Entities;

namespace IFC.Entities
{
    public static class IfcEntityFactory
    {
        public static T CreateEntity<T>(StartAbstractEntity startAbstractEntity, IfcNodeEntity? ifcNodeEntity = null, IfcPipeEntity[]? ifcPipeEntities = null)
            where T : IfcAbstractEntity
        {
            if (ifcNodeEntity == null) return (T)Activator.CreateInstance(typeof(T), startAbstractEntity)!;
            if (ifcPipeEntities == null) return (T)Activator.CreateInstance(typeof(T), startAbstractEntity, ifcNodeEntity)!;
            return (T)Activator.CreateInstance(typeof(T), startAbstractEntity, ifcNodeEntity, ifcPipeEntities)!;
        }
    
        public static T CreateEntity<T>(StartAbstractEntity startAbstractEntity, IfcNodeEntity[] ifcNodeEntities)
            where T : IfcAbstractEntity
        {
            return (T)Activator.CreateInstance(typeof(T), startAbstractEntity, ifcNodeEntities)!;
        }
    
        public static IfcAbstractEntity CreateFittingEntity(StartAbstractEntity startAbstractEntity, IfcNodeEntity? ifcNodeEntity = null, IfcPipeEntity[]? ifcPipeEntities = null)
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
                    return new IfcBendEntity((StartBendEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.MILTER_JOINT:
                    return new IfcMilterJointEntity((StartBendEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.WELDED_TEE:
                    return new IfcWeldedTeeEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.WELDOLET:
                    return new IfcWeldoletEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.SWEEPOLET:
                    return new IfcSweepoletEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.FABRICATED_TEE:
                    return new IfcFabricatedTeeEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.STUB_IN:
                    return new IfcStubInEntity((StartTeeEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.REDUCER_CONCENTRIC:
                    return new IfcReducerConcentricEntity((StartReducerEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.REDUCER_ECCENTRIC:
                    return new IfcReducerEccentricEntity((StartReducerEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.VALVE:
                    return new IfcValveEntity((StartArmatureEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);
            
                case StartElementType.FLANGE:
                    return new IfcFlangeEntity((StartArmatureEntity)startAbstractEntity, ifcNodeEntity, ifcPipeEntities);

                case StartElementType.ALL:
                case StartElementType.GIMBAL_EXPANSION_JOINT:
                case StartElementType.AXIAL_EXPANSION_JOINT:
                case StartElementType.LATERAL_EXPANSION_JOINT:
                case StartElementType.NONSTANDARD_EXPANSION_JOINT:
                case StartElementType.NONSTANDARD_TEE:
                case StartElementType.CAP:
                case StartElementType.EXTRUDED_TEE:
                case StartElementType.UNIVERSAL_EXPANSION_JOINT:
                case StartElementType.TORSION_EXPANSION_JOINT:
                case StartElementType.VESSEL:
                case StartElementType.TANK:
                case StartElementType.PUMP_API_610:
                case StartElementType.TURBINE:
                case StartElementType.COMPRESSOR:
                case StartElementType.AIR_COOLER:
                case StartElementType.FRIED_HEATER:
                case StartElementType.PUMP_ISO_9905:
                case StartElementType.PUMP_ISO_5199:
                case StartElementType.INLINE_PUMP:
                case StartElementType.OTHER_PUMP:
                case StartElementType.ANCHOR:
                case StartElementType.SLIDING_SUPPORT:
                case StartElementType.GUIDE_SINGLE_DIRECTION_SUPPORT:
                case StartElementType.RIGID_HANGER:
                case StartElementType.SPRING_HANGER:
                case StartElementType.SPRING_SUPPORT:
                case StartElementType.HINGED_ANCHOR:
                case StartElementType.CONSTANT_FORCE_SUPPORT:
                case StartElementType.MARKER:
                case StartElementType.GUIDE_DOUBLE_DIRECTION_SUPPORT:
                case StartElementType.COLD_SPRING_PRE_STRETCH:
                case StartElementType.PRE_COMPRESSION:
                case StartElementType.LINEAR_RESTRAINT_MOVEMENT:
                case StartElementType.ANGULAR_RESTRAINT_MOVEMENT:
                case StartElementType.RELATIVE_LINEAR_DISPLACEMENT_IN_THE_NODE:
                case StartElementType.RELATIVE_ROTATIONAL_DISPLACEMENT_IN_THE_NODE:
                case StartElementType.ELASTIC_RESTRAINT:
                case StartElementType.ONE_WAY_RESTRAINT:
                case StartElementType.TWO_WAY_RESTRAINT:
                case StartElementType.BASE_CLASS_OF_TEMPERATURES_CYCLES:
                case StartElementType.TEMPERATURE_CYCLE:
                case StartElementType.SUPPORT_LOAD:
                case StartElementType.NODE_DISPLACEMENT:
                case StartElementType.EXPANSION_JOINT_DEFORMATION:
                case StartElementType.SPRING_HANGER_PROPERTIES:
                case StartElementType.CODE_STRESS:
                default:
                    throw new Exception("Unknown entity type");
            }
        }
    }
}