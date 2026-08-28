using System.Collections.Generic;

namespace IFCConverter.Utils.Registries
{
    public interface IRegistry<in TEntity, TRegistration>
    {
        TRegistration Resolve(TEntity entity);
        IEnumerable<TRegistration> ResolveAll(TEntity source);
        bool TryResolve(TEntity entity, out TRegistration registration);
    }

    public interface IRegistry<in TEntity, TRegistration, in TContext>
    {
        TRegistration Resolve(TEntity entity, TContext context);
        IEnumerable<TRegistration> ResolveAll(TEntity entity, TContext context);
        bool TryResolve(TEntity entity, TContext context, out TRegistration registration);
    }
}