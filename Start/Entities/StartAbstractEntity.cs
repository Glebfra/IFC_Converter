using IFC_Converter.Math;
using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public abstract class StartAbstractEntity : IDisposable
{
    protected StartBaseRoot entity { get; }

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

    public Vector3 GetCoordinates()
    {
        return new Vector3(GetXCoord(), GetYCoord(), GetZCoord());
    }

    public virtual Dictionary<string, string> GetData()
    {
        Dictionary<string, string> dictionary = new()
        {
            { "XCoordinate", GetXCoord().ToString() },
            { "YCoordinate", GetYCoord().ToString() },
            { "ZCoordinate", GetZCoord().ToString() }
        };

        return dictionary;
    }

    public void Dispose()
    {
        entity.Dispose();
    }
}