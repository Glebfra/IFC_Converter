using System.Collections.Generic;

namespace IFCConverter.Domain.Metadata
{
    public class EntityMetadata : IEntityMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string MaterialName { get; set; }
        public string Color { get; set; }
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    }
}