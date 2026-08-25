using System.Collections.Generic;

namespace IFCConverter.Domain.Metadata
{
    public sealed class PortMetadata
    {
        public double Diameter = 0.0;
        public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();
    }
}