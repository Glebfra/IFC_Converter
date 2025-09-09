using IFC.Entities;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
using IFCConverter.Extensions.Entities.Anchors;
using IFCConverter.Extensions.Entities.Fittings;
using IFCConverter.Extensions.Entities.Segments;
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
                    return IfcPipeSegmentEntityExtensions.CreateFromStart((StartPipeEntity)entity, nodeEntities);
                
                case StartElementType.CYLINDRICAL_SHELL:
                    return IfcCylindricalShellEntityExtensions.CreateFromStart((StartPipeEntity)entity, nodeEntities);
                
                case StartElementType.CONE_ELEMENT:
                    return IfcConeElementEntityExtensions.CreateFromStart((StartConeElementEntity)entity, nodeEntities);

                default:
                    return null;
            }
        }

        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.RIGID_ELEMENT:
                    return IfcRigidElementEntityExtensions.CreateFromStart((StartRigidElementEntity)entity, nodeEntities, segmentEntities);
                
                case StartElementType.FLEXIBLE_ELEMENT:
                    return IfcFlexibleSegmentEntityExtensions.CreateFromStart((StartFlexibleElementEntity)entity, nodeEntities, segmentEntities);

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
                    return IfcFixedAnchorEntityExtensions.CreateFromStart((StartAnchorEntity)entity, nodeEntity, segmentEntities);
                
                #endregion
                
                #region Bends
                
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                    return IfcCadBendEntityExtensions.CreateFromStart((StartBendEntity)entity, nodeEntity, segmentEntities);

                case StartElementType.NONSTANDARD_BEND:
                    return IfcCadBendEntityExtensions.CreateFromStart((StartNonStandardBendEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.MILTER_JOINT:
                    return IfcMilterJointEntityExtensions.CreateFromStart((StartBendEntity)entity, nodeEntity, segmentEntities);
                
                #endregion

                #region Tees

                case StartElementType.FABRICATED_TEE:
                    return IfcFabricatedTeeEntityExtensions.CreateFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.STUB_IN:
                    return IfcStubInEntityExtensions.CreateFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.SWEEPOLET:
                    return IfcSweepoletEntityExtensions.CreateFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);

                case StartElementType.NONSTANDARD_TEE:
                    return IfcNonStandardTeeEntityExtensions.CreateFromStart((StartNonstandardTeeEntity)entity, nodeEntity, segmentEntities);

                case StartElementType.STAND_TEE:
                    return IfcStandTeeEntityExtensions.CreateFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.WELDOLET:
                    return IfcWeldoletEntityExtensions.CreateFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.WELDED_TEE:
                    return IfcWeldedTeeEntityExtensions.CreateFromStart((StartTeeEntity)entity, nodeEntity, segmentEntities);

                #endregion

                #region Expansion joints
                
                case StartElementType.UNIVERSAL_EXPANSION_JOINT:
                    return IfcUniversalExpansionJointEntityExtensions.CreateFromStart((StartUniversalExpansionJointEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.NONSTANDARD_EXPANSION_JOINT:
                    return IfcNonstandardExpansionJointEntityExtensions.CreateFromStart((StartNonstandardExpansionJointEntity)entity, nodeEntity, segmentEntities);

                #endregion

                #region Other fittings

                case StartElementType.CAP:
                    return IfcCapEntityExtensions.CreateFromStart((StartCapEntity)entity, nodeEntity, segmentEntities);
                
                case StartElementType.CONNECTOR:
                    return IfcConnectorEntityExtensions.CreateFromStart((StartConnectorEntity)entity, nodeEntity, segmentEntities);

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
                    return IfcAxialExpansionJointEntityExtensions.CreateFromStart((StartAxialExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.GIMBAL_EXPANSION_JOINT:
                    return IfcVertexAngularExpansionJointEntityExtensions.CreateFromStart((StartAngularExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.BALL_EXPANSION_JOINT:
                    return IfcVertexBallExpansionJointEntityExtensions.CreateFromStart((StartBallExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.LATERAL_EXPANSION_JOINT:
                    return IfcVertexLateralExpansionJointEntityExtensions.CreateFromStart((StartLateralExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion
                
                #region Anchors
                
                case StartElementType.CONSTANT_FORCE_SUPPORT:
                    return IfcConstantForceSupportEntityExtensions.CreateFromStart((StartConstantForceSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.CONSTANT_FORCE_SUPPORT_HANGER:
                    return IfcConstantForceSupportHangerEntityExtensions.CreateFromStart((StartConstantForceSupportHangerEntity)entity, nodeEntity, segmentEntities, numSegments);

                case StartElementType.GUIDE_SINGLE_DIRECTION_SUPPORT:
                    return IfcGuideSingleDirectionSupportEntityExtensions.CreateFromStart((StartGuideSingleDirectionSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.GUIDE_DOUBLE_DIRECTION_SUPPORT:
                    return IfcGuideDoubleDirectionSupportEntityExtensions.CreateFromStart((StartGuideDoubleDirectionSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.HINGED_ANCHOR:
                    return IfcHingedAnchorEntityExtensions.CreateFromStart((StartHingedAnchorEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SPRING_SUPPORT:
                    return IfcSpringSupportEntityExtensions.CreateFromStart((StartSpringSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SPRING_HANGER:
                    return IfcSpringHangerEntityExtensions.CreateFromStart((StartSpringSupportEntity)entity, nodeEntity, segmentEntities, numSegments);

                case StartElementType.RIGID_HANGER:
                    return IfcRigidHangerEntityExtensions.CreateFromStart((StartRigidHangerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SLIDING_SUPPORT:
                    return IfcSlidingSupportEntityExtensions.CreateFromStart((StartSlidingSupportEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.NONSTANDARD_RESTRAINT:
                    return IfcNonStandardRestraintEntityExtensions.CreateFromStart((StartNonStandardRestraint)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.DAMPER:
                    return IfcDamperEntityExtensions.CreateFromStart((StartDamperEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion

                #region Bends

                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                    return IfcVertexBendEntityExtensions.CreateFromStart((StartBendEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SADDLE_BEND:
                    return IfcVertexSaddleBendEntityExtensions.CreateFromStart((StartBendEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion

                #region Other fittings

                case StartElementType.FLANGE:
                    return IfcVertexFlangeEntityExtensions.CreateFromStart((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.REDUCER_CONCENTRIC:
                    return IfcVertexReducerConcentricEntityExtensions.CreateFromStart((StartReducerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.REDUCER_ECCENTRIC:
                    return IfcVertexReducerEccentricEntityExtensions.CreateFromStart((StartReducerEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.SINGLE_FLANGE:
                    return IfcVertexSingleFlangeEntityExtensions.CreateFromStart((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.TORSION_EXPANSION_JOINT:
                    return IfcVertexTorsionExpansionJointEntityExtensions.CreateFromStart((StartTorsionExpansionJointEntity)entity, nodeEntity, segmentEntities, numSegments);
                
                case StartElementType.VALVE:
                    return IfcVertexValveEntityExtensions.CreateFromStart((StartArmatureEntity)entity, nodeEntity, segmentEntities, numSegments);

                #endregion

                default:
                    return null;
            }
        }
    }
}