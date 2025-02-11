using System.Text.Json;
using System.Text.Json.Serialization;
using Start.API;

namespace Start.Entities.Abstract;

public abstract class StartAbstractEntity : IDisposable
{
    public StartBaseRoot Entity { get; }
    public int Id { get; }

    protected StartAbstractEntity(StartBaseRoot entity)
    {
        Entity = entity;
        Id = entity.GetNumber();
    }

    public virtual Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>();
    }

    public void Dispose()
    {
        Entity.Dispose();
    }
}