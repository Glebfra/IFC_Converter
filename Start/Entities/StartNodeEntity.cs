using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public sealed class StartNodeEntity : StartAbstractEntity
{
    public StartNodeEntity(StartBaseRoot entity) : base(entity)
    {
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
        var dictionary = base.GetData();
        dictionary.Add("Name", GetName());
        dictionary.Add("Description", GetDescription());
        dictionary.Add("Additional Load from Weight", GetAdditionalLoadFromWeight().ToString("F"));

        return dictionary;
    }
}