using IFC.Entities.Abstract;
using IFC.Entities.Anchors.CAD;
using IFC.Entities.Anchors.Vertex;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using Start.API;
using Start.Entities.Abstract;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Segments;

namespace IFC.Entities
{
    public static class IfcEntityFactory
    {
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.PIPE_ELEMENT:
                    return new IfcPipeEntity((StartPipeEntity)entity, nodeEntities);
                
                case StartElementType.CONE_ELEMENT:
                    return new IfcConeElementEntity((StartConeElementEntity)entity, nodeEntities);
                
                case StartElementType.CYLINDRICAL_SHELL:
                    return new IfcCylindricalShellEntity((StartPipeEntity)entity, nodeEntities);
                
                default:
                    return null;
            }
        }
        
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.FLEXIBLE_ELEMENT:
                    return new IfcFlexibleSegmentEntity((StartFlexibleElementEntity)entity, nodeEntities, segmentEntities);
                
                case StartElementType.RIGID_ELEMENT:
                    return new IfcRigidElementEntity((StartRigidElementEntity)entity, nodeEntities, segmentEntities);
                
                default:
                    return null;
            }
        }
        
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.ANCHOR:
                    return new IfcAnchorEntity((StartAnchorEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.AXIAL_EXPANSION_JOINT:
                    return new IfcAxialExpansionJointEntity((StartAxialExpansionJointEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                case StartElementType.SADDLE_BEND:
                    return new IfcBendEntity((StartBendEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.FABRICATED_TEE:
                    return new IfcFabricatedTeeEntity((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.MILTER_JOINT:
                    return new IfcMilterJointEntity((StartBendEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.NONSTANDARD_EXPANSION_JOINT:
                    return new IfcNonstandardExpansionJointEntity((StartNonstandardExpansionJointEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.STUB_IN:
                    return new IfcStubInEntity((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.SWEEPOLET:
                    return new IfcSweepoletEntity((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.UNIVERSAL_EXPANSION_JOINT:
                    return new IfcUniversalExpansionJointEntity((StartUniversalExpansionJointEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.WELDED_TEE:
                    return new IfcWeldedTeeEntity((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.WELDOLET:
                    return new IfcWeldoletEntity((StartTeeEntity)entity, nodeEntity, segmentEntities);

                default:
                    return null;
            }
        }
        
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            switch (entity.Type)
            {
                case StartElementType.GIMBAL_EXPANSION_JOINT:
                    return new IfcVertexAngularExpansionJointEntity((StartAngularExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.AXIAL_EXPANSION_JOINT:
                case StartElementType.AXIAL_EXPANSION_SLIP_JOINT:
                    return new IfcVertexAxialExpansionJointEntity((StartAxialExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.BALL_EXPANSION_JOINT:
                    return new IfcVertexBallExpansionJointEntity((StartBallExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                case StartElementType.SADDLE_BEND:
                    return new IfcVertexBendEntity((StartBendEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.FLANGE:
                    return new IfcVertexFlangeEntity((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.LATERAL_EXPANSION_JOINT:
                    return new IfcVertexLateralExpansionJointEntity((StartLateralExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.REDUCER_CONCENTRIC:
                    return new IfcVertexReducerConcentricEntity((StartReducerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.REDUCER_ECCENTRIC:
                    return new IfcVertexReducerEccentricEntity((StartReducerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.TORSION_EXPANSION_JOINT:
                    return new IfcVertexTorsionExpansionJointEntity((StartTorsionExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.VALVE:
                    return new IfcVertexValveEntity((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.HINGED_ANCHOR:
                    return new IfcHingedAnchorEntity((StartHingedAnchorEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                default:
                    return null;
            }
        }
    }
}