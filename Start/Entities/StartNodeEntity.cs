using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

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


    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("X Coordinate", GetXCoord().ToString("F5"));
        dictionary.Add("Y Coordinate", GetYCoord().ToString("F5"));
        dictionary.Add("Z Coordinate", GetZCoord().ToString("F5"));
        dictionary.Add("Name", GetName());
        dictionary.Add("Description", GetDescription());
        dictionary.Add("Additional Load from Weight", GetAdditionalLoadFromWeight().ToString("F5"));

        return dictionary;
    }
}