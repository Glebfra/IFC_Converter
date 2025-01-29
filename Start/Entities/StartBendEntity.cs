using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartBendEntity : StartAbstractEntity
{
    public StartBendEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.BEND_NAME);
    public double GetWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.BEND_WEIGHT);
    public int GetManufacturingTechnology() => Entity.GetDataInt(StartBaseRootFunctionKey.BEND_MANUFACTURING_TECHNOLOGY);
    public double GetWallThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.BEND_WALL_THICKNESS);
    public double GetMillTolerance() => Entity.GetDataReal(StartBaseRootFunctionKey.BEND_MILL_TOLERANCE);
    public double GetMillToleranceOutside() => Entity.GetDataReal(StartBaseRootFunctionKey.BEND_MILL_TOLERANCE_OUTSIDE);
    public double GetRadius() => Entity.GetDataReal(StartBaseRootFunctionKey.BEND_RADIUS);
    public double GetOvalizationCoefficient() => Entity.GetDataReal(StartBaseRootFunctionKey.BEND_OVALIZATION_COEFFICIENT);
    public int GetNumberOfMilters() => Entity.GetDataInt(StartBaseRootFunctionKey.BEND_NUMBER_OF_MILTERS);

    public override Dictionary<string, string> GetData()
    {
        var data = base.GetData();
        data.Add("Name", GetName());
        data.Add("Weight", GetWeight().ToString("F"));
        data.Add("Manufacturing Technology", GetManufacturingTechnology().ToString());
        data.Add("Wall Thickness", GetWallThickness().ToString("F"));
        data.Add("Mill Tolerance", GetMillTolerance().ToString("F"));
        data.Add("Mill Tolerance Outside", GetMillToleranceOutside().ToString("F"));
        data.Add("Radius", GetRadius().ToString("F"));
        data.Add("Ovalization Coefficient", GetOvalizationCoefficient().ToString("F"));
        data.Add("Number of Milters", GetNumberOfMilters().ToString());

        return data;
    }
}