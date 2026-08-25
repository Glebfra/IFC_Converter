using IFCConverter.Domain.Identity;

namespace IFCConverter.Domain.Entities
{
    public abstract class Fitting : Entity
    {
        protected Fitting(EntityId id) : base(id)
        {
        }
    }
}