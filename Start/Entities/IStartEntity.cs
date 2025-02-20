#region

using System.Collections.Generic;

#endregion

namespace Start.Entities
{
    public interface IStartEntity
    {
        public Dictionary<string, string> GetData();
    }
}