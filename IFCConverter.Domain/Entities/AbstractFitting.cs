using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Entities
{
    public abstract class AbstractFitting : Entity
    {
        protected AbstractFitting(EntityId id) : base(id)
        {
        }
        
        public FixedVector<Dim3> Position { get; set; }

        public override IReadOnlyCollection<FixedVector<Dim3>> Positions => new[]
        {
            Position
        };
    }
    
    public abstract class AbstractFitting<TMetadata> : AbstractFitting
        where TMetadata : EntityMetadata
    {
        protected AbstractFitting(EntityId id) : base(id)
        {
        }
        
        public abstract TMetadata TypedMetadata { get; }
        public override EntityMetadata Metadata => TypedMetadata;
    }
}