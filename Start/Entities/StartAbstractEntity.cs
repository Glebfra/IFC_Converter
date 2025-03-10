using System.Collections.Generic;
using Start.API;

namespace Start.Entities
{
    public abstract class StartAbstractEntity
    {
        public abstract Dictionary<string, string> GetData();
        public StartElementType Type = StartElementType.ALL;
    }
}