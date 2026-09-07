using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Entities
{
    public sealed class Anchor : AbstractFitting
    {
        public Anchor(EntityId id) : base(id)
        {
            Port = CreatePort();
        }

        public Port Port { get; }

        public List<AnchorRestraint> Restraints { get; } = new List<AnchorRestraint>();
    }
}