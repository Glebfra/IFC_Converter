using System.Collections.Generic;
using IFCConverter.Domain.Identity;
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
}