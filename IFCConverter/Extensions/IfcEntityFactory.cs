using IFC.Entities;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
using IFCConverter.Extensions.Entities;
using Start.API;
using Start.Entities.Abstract;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Segments;

namespace IFCConverter.Extensions
{
    internal static class IfcEntityFactory
    {
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.PIPE_ELEMENT:
                    return IfcEntitiesExtensions.CreatePipeSegmentFromStart((StartPipeEntity)entity, nodeEntities);
                
                case StartElementType.CYLINDRICAL_SHELL:
                    return IfcEntitiesExtensions.CreateCylindricalShellFromStart((StartPipeEntity)entity, nodeEntities);
                
                case StartElementType.CONE_ELEMENT:
                    return IfcEntitiesExtensions.CreateConeElementFromStart((StartConeElementEntity)entity, nodeEntities);

                default:
                    return null;
            }
        }

        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.RIGID_ELEMENT:
                    return IfcEntitiesExtensions.CreateRigidElementFromStart((StartRigidElementEntity)entity, nodeEntities, segmentEntities);
                
                case StartElementType.FLEXIBLE_ELEMENT:
                    return IfcEntitiesExtensions.CreateFlexibleSegmentFromStart((StartFlexibleElementEntity)entity, nodeEntities, segmentEntities);

                default:
                    return null;
            }
        }
        
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            switch (entity.Type)
            {
                #region Anchors
                
                case StartElementType.ANCHOR:
                    return IfcEntitiesExtensions.CreateFixedAnchorFromStart((StartAnchorEntity)entity, nodeEntity, segmentEntities);
                
                #endregion
                
                #region Bends
                
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                    return IfcEntitiesExtensions.CreateCadBendFromStart((StartBendEntity)entity, nodeEntity, segmentEntities);

                case StartElementType.NONSTANDARD_BEND:
                    return IfcEntitiesExtensions.CreateCadBendFromStart((StartNonStandardBendEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.MILTER_JOINT:
                    return IfcEntitiesExtensions.CreateMilterJointFromStart((StartBendEntity)entity, nodeEntity, segmentEntities);
                
                #endregion

                #region Tees

                case StartElementType.FABRICATED_TEE:
                    return IfcEntitiesExtensions.CreateFabricatedTeeFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.STUB_IN:
                    return IfcEntitiesExtensions.CreateStubInFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.SWEEPOLET:
                    return IfcEntitiesExtensions.CreateSweepoletFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);

                case StartElementType.NONSTANDARD_TEE:
                    return IfcEntitiesExtensions.CreateNonstandardTeeFromStart((StartNonstandardTeeEntity)entity, nodeEntity, segmentEntities);

                case StartElementType.STAND_TEE:
                    return IfcEntitiesExtensions.CreateStandTeeFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.WELDOLET:
                    return IfcEntitiesExtensions.CreateWeldoletFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.WELDED_TEE:
                    return IfcEntitiesExtensions.CreateWeldedTeeFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);

                #endregion

                #region Expansion joints
                
                case StartElementType.UNIVERSAL_EXPANSION_JOINT:
                    return IfcEntitiesExtensions.CreateUniversalExpansionJointFromStart((StartUniversalExpansionJointEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.NONSTANDARD_EXPANSION_JOINT:
                    return IfcEntitiesExtensions.CreateNonstandardExpansionFromStart((StartNonstandardExpansionJointEntity)entity, nodeEntity, segmentEntities);

                #endregion

                #region Other fittings

                case StartElementType.CAP:
                    return IfcEntitiesExtensions.CreateCapFromStart((StartCapEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.CONNECTOR:
                    return IfcEntitiesExtensions.CreateConnectorFromStart((StartConnectorEntity)entity, nodeEntity, segmentEntities);

                #endregion

                default:
                    return null;
            }
        }
        
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            switch (entity.Type)
            {
                #region Expansion Joints
                
                case StartElementType.AXIAL_EXPANSION_JOINT:
                case StartElementType.AXIAL_EXPANSION_SLIP_JOINT:
                    return IfcEntitiesExtensions.CreateAxialExpansionJointFromStart((StartAxialExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.GIMBAL_EXPANSION_JOINT:
                    return IfcEntitiesExtensions.CreateAngularExpansionJointFromStart((StartAngularExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.BALL_EXPANSION_JOINT:
                    return IfcEntitiesExtensions.CreateBallExpansionJointFromStart((StartBallExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.LATERAL_EXPANSION_JOINT:
                    return IfcEntitiesExtensions.CreateLateralExpansionJointFromStart((StartLateralExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.TORSION_EXPANSION_JOINT:
                    return IfcEntitiesExtensions.CreateTorsionExpansionJointFromStart((StartTorsionExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion
                
                #region Anchors
                
                case StartElementType.CONSTANT_FORCE_SUPPORT:
                    return IfcEntitiesExtensions.CreateConstantForceSupportFromStart((StartConstantForceSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.CONSTANT_FORCE_SUPPORT_HANGER:
                    return IfcEntitiesExtensions.CreateConstantForceHangerFromStart((StartConstantForceSupportHangerEntity)entity, nodeEntity, segmentEntities, numSegments);

                case StartElementType.GUIDE_SINGLE_DIRECTION_SUPPORT:
                    return IfcEntitiesExtensions.CreateGuideSingleDirectionFromStart((StartGuideSingleDirectionSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.GUIDE_DOUBLE_DIRECTION_SUPPORT:
                    return IfcEntitiesExtensions.CreateGuideDoubleDirectionFromStart((StartGuideDoubleDirectionSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.HINGED_ANCHOR:
                    return IfcEntitiesExtensions.CreateHingedAnchorFromStart((StartHingedAnchorEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SPRING_SUPPORT:
                    return IfcEntitiesExtensions.CreateSpringSupportFromStart((StartSpringSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SPRING_HANGER:
                    return IfcEntitiesExtensions.CreateSpringHangerFromStart((StartSpringSupportEntity)entity, nodeEntity, segmentEntities, numSegments);

                case StartElementType.RIGID_HANGER:
                    return IfcEntitiesExtensions.CreateRigidHangerFromStart((StartRigidHangerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SLIDING_SUPPORT:
                    return IfcEntitiesExtensions.CreateSlidingSupportFromStart((StartSlidingSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.NONSTANDARD_RESTRAINT:
                    return IfcEntitiesExtensions.CreateNonStandardRestraintFromStart((StartNonStandardRestraint)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.DAMPER:
                    return IfcEntitiesExtensions.CreateDamperFromStart((StartDamperEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion

                #region Bends

                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                    return IfcEntitiesExtensions.CreateVertexBendFromStart((StartBendEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SADDLE_BEND:
                    return IfcEntitiesExtensions.CreateSaddleBendFromStart((StartBendEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion

                #region Other fittings

                case StartElementType.FLANGE:
                    return IfcEntitiesExtensions.CreateFlangeFromStart((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.REDUCER_CONCENTRIC:
                    return IfcEntitiesExtensions.CreateReducerConcentricFromStart((StartReducerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.REDUCER_ECCENTRIC:
                    return IfcEntitiesExtensions.CreateReducerEccentricFromStart((StartReducerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SINGLE_FLANGE:
                    return IfcEntitiesExtensions.CreateSingleFlangeFromStart((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);

                case StartElementType.VALVE:
                    return IfcEntitiesExtensions.CreateValveFromStart((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion

                default:
                    return null;
            }
        }
    }
}