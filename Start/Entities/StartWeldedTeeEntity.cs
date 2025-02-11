using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public class StartWeldedTeeEntity : StartAbstractTeeEntity
{
    public StartWeldedTeeEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public double GetCrotchRadius() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_RADIUS);
    public double GetCrotchThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_THICKNESS);
    public double GetCrotchHeight() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_HEIGHT);

    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("Crotch radius", GetCrotchRadius().ToString("F"));
        dictionary.Add("Crotch thickness", GetCrotchThickness().ToString("F"));
        dictionary.Add("Crotch height", GetCrotchHeight().ToString("F"));

        return dictionary;
    }
}