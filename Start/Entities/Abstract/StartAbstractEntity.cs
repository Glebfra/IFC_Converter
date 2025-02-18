#region

using Start.API;

#endregion

namespace Start.Entities.Abstract;

public abstract class StartAbstractEntity : IDisposable
{
    public readonly StartBaseRoot Entity;
    public readonly int Id;
    
    protected StartAbstractEntity(StartBaseRoot entity)
    {
        Entity = entity;
        Id = entity.GetNumber();
    }

    public void Dispose()
    {
        Entity.Dispose();
    }
}