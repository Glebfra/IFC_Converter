using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities.Abstract
{
    public abstract class StartAbstractEntity
    {
        [JsonIgnore]
        public int ID { get; set; }
        
        [JsonIgnore]
        public StartElementType Type { get; set; } = StartElementType.ALL;
        
        public abstract Dictionary<string, string> GetData();
    }
}