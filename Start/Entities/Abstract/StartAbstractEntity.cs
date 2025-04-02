using System.Collections.Generic;
using Start.API;

namespace Start.Entities.Abstract
{
    public abstract class StartAbstractEntity
    {
        public abstract Dictionary<string, string> GetData();
        public int ID;
        public StartElementType Type = StartElementType.ALL;
    }
}