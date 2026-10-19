using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Entities
{
    public abstract class AbstractSegment : Entity
    {
        protected AbstractSegment(EntityId id) : base(id)
        {
            StartPort = CreatePort();
            EndPort = CreatePort();
        }

        public Port StartPort { get; }
        public Port EndPort { get; }

        public override IReadOnlyCollection<FixedVector<Dim3>> Positions => new[]
        {
            StartPort.Position, EndPort.Position
        };
    }
    
    public abstract class AbstractSegment<TMetadata> : AbstractSegment, IEntityTypedMetadata<TMetadata>
        where TMetadata : EntityMetadata
    {
        protected AbstractSegment(EntityId id) : base(id)
        {
        }
        
        public abstract TMetadata TypedMetadata { get; }

        public override EntityMetadata Metadata => TypedMetadata;
    }
}