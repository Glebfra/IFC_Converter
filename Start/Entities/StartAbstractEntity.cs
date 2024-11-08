using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public abstract class StartAbstractEntity : IDisposable
{
    public StartBaseRoot entity { get; }
    public StartElementType elementType { get; protected set; }

    public StartAbstractEntity(StartBaseRoot entity)
    {
        this.entity = entity;
    }

    public void Dispose()
    {
        entity.Dispose();
    }
}