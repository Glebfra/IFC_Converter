#region

using Start.API;

#endregion

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

    public int GetNumberConn() => Entity.GetNumberConn();
    public int GetNumber() => Entity.GetNumber();

    public StartBaseRoot GetConnElementOnType(StartElementType type, int id) => Entity.GetConnElemOnType(type, id);
    public StartBaseRoot GetConnElementOnIndex(int id) => Entity.GetConnElemOnIndex(id);

    public virtual Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>();
    }

    public void Dispose()
    {
        Entity.Dispose();
    }
}