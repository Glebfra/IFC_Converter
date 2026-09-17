using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Entities
{
    public sealed class PumpApi610 : Equipment
    {

        public PumpApi610(EntityId id) : base(id)
        {
            SecondPortA = CreatePort();
            SecondPortB = CreatePort();
        }

        public FixedVector<Dim3> SecondPosition { get; set; }

        public Port SecondPortA { get; }
        public Port SecondPortB { get; }

        public override IReadOnlyCollection<FixedVector<Dim3>> Positions => new[]
        {
            Position, SecondPosition
        };
    }
}