using System.Collections.Generic;
using IFCConverter.Domain.Identity;
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
}