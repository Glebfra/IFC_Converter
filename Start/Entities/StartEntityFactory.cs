using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;
using Start.Entities.Anchors;
using Start.Entities.Fittings;
using Start.Entities.Segments;

namespace Start.Entities
{
    public static class StartEntityFactory
    {
        public static StartAbstractEntity? CreateEntity(StartDataArrayItem dataArrayItem)
        {
            switch (dataArrayItem.Type)
            {
                case StartElementType.NODE:
                    return JsonConvert.DeserializeObject<StartNodeEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.PIPE_ELEMENT:
                case StartElementType.CYLINDRICAL_SHELL:
                    return JsonConvert.DeserializeObject<StartPipeEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.ANCHOR:
                    return JsonConvert.DeserializeObject<StartAnchorEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.CONE_ELEMENT:
                    return JsonConvert.DeserializeObject<StartConeElementEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.GIMBAL_EXPANSION_JOINT:
                    return JsonConvert.DeserializeObject<StartAngularExpansionJointEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.VALVE:
                case StartElementType.FLANGE:
                    return JsonConvert.DeserializeObject<StartArmatureEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.AXIAL_EXPANSION_JOINT:
                case StartElementType.AXIAL_EXPANSION_SLIP_JOINT:
                    return JsonConvert.DeserializeObject<StartAxialExpansionJointEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.BALL_EXPANSION_JOINT:
                    return JsonConvert.DeserializeObject<StartBallExpansionJointEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                case StartElementType.SADDLE_BEND:
                case StartElementType.MILTER_JOINT:
                    return JsonConvert.DeserializeObject<StartBendEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.FLEXIBLE_ELEMENT:
                    return JsonConvert.DeserializeObject<StartFlexibleElementEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.LATERAL_EXPANSION_JOINT:
                    return JsonConvert.DeserializeObject<StartLateralExpansionJointEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.NONSTANDARD_EXPANSION_JOINT:
                    return JsonConvert.DeserializeObject<StartNonstandardExpansionJointEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.REDUCER_CONCENTRIC:
                case StartElementType.REDUCER_ECCENTRIC:
                    return JsonConvert.DeserializeObject<StartReducerEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.RIGID_ELEMENT:
                    return JsonConvert.DeserializeObject<StartRigidElementEntity>(dataArrayItem.Data.ToString());

                case StartElementType.WELDED_TEE:
                case StartElementType.WELDOLET:
                case StartElementType.SWEEPOLET:
                case StartElementType.FABRICATED_TEE:
                case StartElementType.STUB_IN:
                    return JsonConvert.DeserializeObject<StartTeeEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.TORSION_EXPANSION_JOINT:
                    return JsonConvert.DeserializeObject<StartTorsionExpansionJointEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.UNIVERSAL_EXPANSION_JOINT:
                    return JsonConvert.DeserializeObject<StartUniversalExpansionJointEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.HINGED_ANCHOR:
                    return JsonConvert.DeserializeObject<StartHingedAnchorEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.SLIDING_SUPPORT:
                    return JsonConvert.DeserializeObject<StartSlidingSupportEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.SPRING_SUPPORT:
                case StartElementType.SPRING_HANGER:
                    return JsonConvert.DeserializeObject<StartSpringSupportEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.GUIDE_SINGLE_DIRECTION_SUPPORT:
                    return JsonConvert.DeserializeObject<StartGuideSingleDirectionSupportEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.GUIDE_DOUBLE_DIRECTION_SUPPORT:
                    return JsonConvert.DeserializeObject<StartGuideDoubleDirectionSupportEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.RIGID_HANGER:
                    return JsonConvert.DeserializeObject<StartRigidHangerEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.CONSTANT_FORCE_SUPPORT:
                    return JsonConvert.DeserializeObject<StartConstantForceSupportEntity>(dataArrayItem.Data.ToString());
                
                case StartElementType.CONSTANT_FORCE_SUPPORT_HANGER:
                    return JsonConvert.DeserializeObject<StartConstantForceSupportHangerEntity>(dataArrayItem.Data.ToString());
                
                default:
                    return null;
            }
        }
    }
}