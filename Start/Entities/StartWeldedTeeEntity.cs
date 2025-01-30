using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartWeldedTeeEntity : StartAbstractEntity
{
    public StartWeldedTeeEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public StartNodeEntity GetConnNode() =>
        new StartNodeEntity(Entity.GetConnElemOnType(StartElementType.PIPE_ELEMENT, 0));

    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.WELDED_TEE_NAME);
    public double GetHeaderThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_HEADER_THICKNESS);
    public double GetMillTolerance() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_MILL_TOLERANCE);
    public double GetWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_WEIGHT);
    public double GetHeaderLength() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_HEADER_LENGTH);
    public double GetStrengthFactorOfLongitudinalWeldSeamOnPressure() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_STRENGTH_FACTOR_OF_LONGITUDINAL_WELD_SEAM_ON_PRESSURE);
    public double GetBranchHeight() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_BRANCH_HEIGHT);
    public double GetCrotchRadius() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_RADIUS);
    public double GetCrotchThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_THICKNESS);
    public double GetCrotchHeight() => Entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_HEIGHT);

    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("Header thickness", GetHeaderThickness().ToString("F"));
        dictionary.Add("Mill tolerance", GetMillTolerance().ToString("F"));
        dictionary.Add("Weight", GetWeight().ToString("F"));
        dictionary.Add("Header length", GetHeaderLength().ToString("F"));
        dictionary.Add("Strength factor of longitudinal weld seam on pressure",
            GetStrengthFactorOfLongitudinalWeldSeamOnPressure().ToString("F"));
        dictionary.Add("Branch height", GetBranchHeight().ToString("F"));
        dictionary.Add("Crotch radius", GetCrotchRadius().ToString("F"));
        dictionary.Add("Crotch thickness", GetCrotchThickness().ToString("F"));
        dictionary.Add("Crotch height", GetCrotchHeight().ToString("F"));
        dictionary.Add("Name", GetName());

        return dictionary;
    }
}