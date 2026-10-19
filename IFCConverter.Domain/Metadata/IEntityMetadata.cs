using System.Collections.Generic;

namespace IFCConverter.Domain.Metadata
{
    public interface IEntityMetadata
    {
        string Name { get; set; }
        string Description { get; set; }
        string Type { get; set; }
        string MaterialName { get; set; }
        string Color { get; set; }
        IDictionary<string, object> Properties { get; }
    }
}