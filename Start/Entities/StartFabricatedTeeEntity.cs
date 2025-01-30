using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartFabricatedTeeEntity : StartAbstractEntity
{
    public StartFabricatedTeeEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public double GetHeaderThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_HEADER_THICKNESS);
    public double GetMillToleranceForHeader() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_MILL_TOLERANCE_FOR_HEADER);
    public double GetWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_WEIGHT);
    public double GetHeaderLength() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_HEADER_LENGTH);
    public double GetBranchWallThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_BRANCH_WALL_THICKNESS);
    public double GetMillToleranceForBranch() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_MILL_TOLERANCE_FOR_BRANCH);
    public double GetStrengthFactorOfLongitudinalWeldSeamOnPressure() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_STRENGTH_FACTOR_OF_LONGITUDINAL_WELD_SEAM_ON_PRESSURE);
    public double GetBranchHeight() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_BRANCH_HEIGHT);
    public double GetPadThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_PAD_THICKNESS);
    public double GetPadWidth() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_PAD_WIDTH);
    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.FABRICATED_TEE_NAME);

    public override Dictionary<string, string> GetData()
    {
        var data = base.GetData();
        data.Add("Header Thickness", GetHeaderThickness().ToString("F"));
        data.Add("Mill Tolerance For Header", GetMillToleranceForHeader().ToString("F"));
        data.Add("Weight", GetWeight().ToString("F"));
        data.Add("Header Length", GetHeaderLength().ToString("F"));
        data.Add("Branch Wall Thickness", GetBranchWallThickness().ToString("F"));
        data.Add("Mill Tolerance For Branch", GetMillToleranceForBranch().ToString("F"));
        data.Add("Strength Factor Of Longitudinal Weld Seam On Pressure", GetStrengthFactorOfLongitudinalWeldSeamOnPressure().ToString("F"));
        data.Add("Branch Height", GetBranchHeight().ToString("F"));
        data.Add("Pad Thickness", GetPadThickness().ToString("F"));
        data.Add("Pad Width", GetPadWidth().ToString("F"));
        data.Add("Name", GetName());

        return data;
    }
}