using System.Collections.Generic;

namespace IFCConverter.Utils
{
    public interface IRegistry<in TEntity, TRegistration>
    {
        public TRegistration Resolve(TEntity entity);
        public IEnumerable<TRegistration> ResolveAll(TEntity source);
        public bool TryResolve(TEntity entity, out TRegistration registration);
    }

    public interface IRegistry<in TEntity, TRegistration, in TContext>
    {
        public TRegistration Resolve(TEntity entity, TContext context);
        public IEnumerable<TRegistration> ResolveAll(TEntity entity, TContext context);
        public bool TryResolve(TEntity entity, TContext context, out TRegistration registration);
    }
}