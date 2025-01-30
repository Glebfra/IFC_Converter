using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFC_Converter.Start.Entities;

public sealed class StartNodeEntity : StartAbstractEntity
{
    public StartNodeEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public double GetAdditionalLoadFromWeight() =>
        Entity.GetDataReal(StartBaseRootFunctionKey.NODE_ADDITIONAL_LOAD_FROM_WEIGHT);

    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.NODE_NAME);
    public string GetDescription() => Entity.GetDataChar(StartBaseRootFunctionKey.NODE_DESCRIPTION);
    
    public double GetXCoord() => Entity.GetXCoord();
    public double GetYCoord() => Entity.GetYCoord();
    public double GetZCoord() => Entity.GetZCoord();
    
    public XbimVector3D GetCoordinates() => new(GetXCoord(), GetYCoord(), GetZCoord());


    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("X Coordinate", GetXCoord().ToString("F"));
        dictionary.Add("Y Coordinate", GetYCoord().ToString("F"));
        dictionary.Add("Z Coordinate", GetZCoord().ToString("F"));
        dictionary.Add("Name", GetName());
        dictionary.Add("Description", GetDescription());
        dictionary.Add("Additional Load from Weight", GetAdditionalLoadFromWeight().ToString("F"));

        return dictionary;
    }
}