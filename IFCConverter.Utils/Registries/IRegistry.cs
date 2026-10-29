using System.Collections.Generic;

namespace IFCConverter.Utils.Registries
{
    public interface IRegistry<in TEntity, TRegistration>
    {
        TRegistration Resolve(TEntity entity);
        IEnumerable<TRegistration> ResolveAll(TEntity source);
        bool TryResolve(TEntity entity, out TRegistration registration);
    }
}