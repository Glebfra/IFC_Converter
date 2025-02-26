using Newtonsoft.Json;
using Start.API;

namespace Start.Entities
{
    public static class StartEntityFactory
    {
        public static StartAbstractEntity? CreateEntity(StartDataArrayItem arrayItem)
        {
            StartAbstractEntity abstractEntity;

            switch (arrayItem.Type)
            {
                case StartElementType.PIPE_ELEMENT:
                    abstractEntity = JsonConvert.DeserializeObject<StartPipeEntity>(arrayItem.Data.ToString())!;
                    break;
            
                case StartElementType.NODE:
                    abstractEntity = JsonConvert.DeserializeObject<StartNodeEntity>(arrayItem.Data.ToString())!; 
                    break;
            
                case StartElementType.ELBOW:
                case StartElementType.PIPE_BEND:
                case StartElementType.MILTER_BEND:
                case StartElementType.WELDED_BEND:
                case StartElementType.LONG_RADIUS_PIPE_BEND:
                case StartElementType.PRE_STRESSED_PIPE_BEND:
                case StartElementType.SADDLE_BEND:
                case StartElementType.MILTER_JOINT:
                    abstractEntity = JsonConvert.DeserializeObject<StartBendEntity>(arrayItem.Data.ToString())!;
                    break;
            
                case StartElementType.WELDED_TEE:
                case StartElementType.WELDOLET:
                case StartElementType.SWEEPOLET:
                case StartElementType.FABRICATED_TEE:
                case StartElementType.STUB_IN:
                    abstractEntity = JsonConvert.DeserializeObject<StartTeeEntity>(arrayItem.Data.ToString())!;
                    break;
            
                case StartElementType.REDUCER_CONCENTRIC:
                case StartElementType.REDUCER_ECCENTRIC:
                    abstractEntity = JsonConvert.DeserializeObject<StartReducerEntity>(arrayItem.Data.ToString())!;
                    break;
            
                case StartElementType.VALVE:
                case StartElementType.FLANGE:
                    abstractEntity = JsonConvert.DeserializeObject<StartArmatureEntity>(arrayItem.Data.ToString())!;
                    break;
            
                default:
                    return null;
            }
        
            abstractEntity.Type = arrayItem.Type;
            return abstractEntity;
        }
    }
}