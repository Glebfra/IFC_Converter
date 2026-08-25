namespace IFCConverter.Utils
{
    public interface IRegistry<in TEntity, TRegistration>
    {
        public TRegistration Resolve(TEntity entity);
        public bool TryResolve(TEntity entity, out TRegistration exporter);
    }
}