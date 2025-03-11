using Newtonsoft.Json;
using Start.Entities;

namespace Start.API
{
    public class StartDataArrayItem
    {
        [JsonProperty("nodeIds")]
        public int[] NodeIds { get; set; }

        [JsonProperty("typeId")]
        public StartElementType Type { get; set; }
    
        [JsonProperty("dataArrayIndex")]
        public int DataArrayIndex { get; set; }
    
        [JsonProperty("dataArrayUIndex")]
        public int DataArrayUIndex { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        public StartAbstractEntity Entity;
    }
}