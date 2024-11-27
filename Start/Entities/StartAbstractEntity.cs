using System.Text.Json.Serialization;
using IFC_Converter.Math;
using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public abstract class StartAbstractEntity : IDisposable
{
    public StartBaseRoot entity { get; }
    public int Id { get; }

    public StartAbstractEntity(StartBaseRoot entity)
    {
        this.entity = entity;
        Id = entity.GetNumber();
    }

    public StartBaseRoot GetEntity()
    {
        return entity;
    }

    public int GetNumberConn()
    {
        return entity.GetNumberConn();
    }

    public StartBaseRoot GetConnElementOnType(StartElementType type, int id)
    {
        return entity.GetConnElemOnType(type, id);
    }

    public StartBaseRoot GetConnElementOnIndex(int id)
    {
        return entity.GetConnElemOnIndex(id);
    }

    public int GetNumber()
    {
        return entity.GetNumber();
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
            { "XCoordinate", GetXCoord().ToString("F") },
            { "YCoordinate", GetYCoord().ToString("F") },
            { "ZCoordinate", GetZCoord().ToString("F") }
        };

        return dictionary;
    }

    public void Dispose()
    {
        entity.Dispose();
    }
}