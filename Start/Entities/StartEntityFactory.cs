using Newtonsoft.Json;
using Start.API;

namespace Start.Entities
{
    public static class StartEntityFactory
    {
        public static StartAbstractEntity? CreateEntity(StartDataArrayItem arrayItem)
        {
            StartAbstractEntity abstractEntity;
            string dataString = arrayItem.Data.ToString();

            switch (arrayItem.Type)
            {
                case StartElementType.PIPE_ELEMENT:
                case StartElementType.CYLINDRICAL_SHELL:
                    abstractEntity = JsonConvert.DeserializeObject<StartPipeEntity>(dataString)!;
                    break;
                
                case StartElementType.FLEXIBLE_ELEMENT:
                    abstractEntity = JsonConvert.DeserializeObject<StartFlexibleElementEntity>(dataString)!;
                    break;
                
                case StartElementType.CONE_ELEMENT:
                    abstractEntity = JsonConvert.DeserializeObject<StartConeElementEntity>(dataString)!;
                    break;

                case StartElementType.NODE:
                    abstractEntity = JsonConvert.DeserializeObject<StartNodeEntity>(dataString)!; 
                    break;
                
                case StartElementType.RIGID_ELEMENT:
                    abstractEntity = JsonConvert.DeserializeObject<StartRigidElementEntity>(dataString)!;
                    break;
                
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                case StartElementType.SADDLE_BEND:
                case StartElementType.MILTER_JOINT:
                    abstractEntity = JsonConvert.DeserializeObject<StartBendEntity>(dataString)!;
                    break;
            
                case StartElementType.WELDED_TEE:
                case StartElementType.WELDOLET:
                case StartElementType.SWEEPOLET:
                case StartElementType.FABRICATED_TEE:
                case StartElementType.STUB_IN:
                    abstractEntity = JsonConvert.DeserializeObject<StartTeeEntity>(dataString)!;
                    break;
            
                case StartElementType.REDUCER_CONCENTRIC:
                case StartElementType.REDUCER_ECCENTRIC:
                    abstractEntity = JsonConvert.DeserializeObject<StartReducerEntity>(dataString)!;
                    break;
            
                case StartElementType.VALVE:
                case StartElementType.FLANGE:
                    abstractEntity = JsonConvert.DeserializeObject<StartArmatureEntity>(dataString)!;
                    break;
            
                default:
                    return null;
            }
        
            abstractEntity.Type = arrayItem.Type;
            return abstractEntity;
        }
    }
}