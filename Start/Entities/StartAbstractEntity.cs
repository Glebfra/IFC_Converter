using IFC_Converter.Start.API;
using Xbim.Common.Geometry;

namespace IFC_Converter.Start.Entities;

public abstract class StartAbstractEntity : IDisposable
{
    public StartBaseRoot Entity { get; }
    public int Id { get; }

    public StartAbstractEntity(StartBaseRoot entity)
    {
        Entity = entity;
        Id = entity.GetNumber();
    }

    public int GetNumberConn() => Entity.GetNumberConn();
    public int GetNumber() => Entity.GetNumber();

    public StartBaseRoot GetConnElementOnType(StartElementType type, int id) => Entity.GetConnElemOnType(type, id);
    public StartBaseRoot GetConnElementOnIndex(int id) => Entity.GetConnElemOnIndex(id);

    public double GetXCoord() => Entity.GetXCoord();
    public double GetYCoord() => Entity.GetYCoord();
    public double GetZCoord() => Entity.GetZCoord();

    public XbimVector3D GetCoordinates() => new(GetXCoord(), GetYCoord(), GetZCoord());

    public virtual Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>()
        {
            { "XCoordinate", GetXCoord().ToString("F") },
            { "YCoordinate", GetYCoord().ToString("F") },
            { "ZCoordinate", GetZCoord().ToString("F") }
        };
    }

    public void Dispose()
    {
        Entity.Dispose();
    }
}