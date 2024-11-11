using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartNodeEntity : StartAbstractEntity
{
    public StartNodeEntity(StartBaseRoot entity) : base(entity)
    {
        elementType = StartElementType.NODE;
    }

    public double GetAdditionalLoadFromWeight()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.NODE_ADDITIONAL_LOAD_FROM_WEIGHT);
    }

    public string GetName()
    {
        return entity.GetDataChar(StartBaseRootFunctionKey.NODE_NAME);
    }

    public string GetDescription()
    {
        return entity.GetDataChar(StartBaseRootFunctionKey.NODE_DESCRIPTION);
    }

    public override Dictionary<string, string> GetData()
    {
        Dictionary<string, string> dictionary = new()
        {
            { "Name", GetName() },
            { "Description", GetDescription() },
            { "XCoordinate", GetXCoord().ToString() },
            { "YCoordinate", GetYCoord().ToString() },
            { "ZCoordinate", GetZCoord().ToString() },
            { "Additional Load from Weight", GetAdditionalLoadFromWeight().ToString() }
        };
        return dictionary;
    }
}