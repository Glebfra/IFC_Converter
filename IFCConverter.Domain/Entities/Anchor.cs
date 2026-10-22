using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain.Entities
{
    public sealed class Anchor : AbstractFitting<AnchorMetadata>
    {
        public Anchor(Id<EntityId> id) : base(id)
        {
            Port = CreatePort();
        }

        public Port Port { get; }

        public List<AnchorRestraint> Restraints { get; } = new List<AnchorRestraint>();
        public override AnchorMetadata TypedMetadata { get; } = new AnchorMetadata();
    }
}