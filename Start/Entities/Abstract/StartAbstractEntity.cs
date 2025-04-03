using System.Collections.Generic;
using Start.API;

namespace Start.Entities.Abstract
{
    public abstract class StartAbstractEntity
    {
        public int ID { get; set; }
        public StartElementType Type { get; set; } = StartElementType.ALL;
        
        public abstract Dictionary<string, string> GetData();
    }
}