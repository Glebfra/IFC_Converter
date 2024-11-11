using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public abstract class StartAbstractEntity : IDisposable
{
    protected StartBaseRoot entity { get; }
    public StartElementType elementType { get; protected set; }
    public abstract Dictionary<string, string> GetData();

    public StartAbstractEntity(StartBaseRoot entity)
    {
        this.entity = entity;
    }
    
    public double GetXCoord()
    {
        return entity.GetXCoord();
    }
    
    public double GetYCoord()
    {
        return entity.GetYCoord();
    }
    
    public double GetZCoord()
    {
        return entity.GetZCoord();
    }

    public void Dispose()
    {
        entity.Dispose();
    }
}