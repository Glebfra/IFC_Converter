using Start.API;

namespace Start.Entities.Abstract;

public abstract class StartAbstractTeeEntity : StartAbstractEntity
{
    protected StartAbstractTeeEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.TEE_NAME);
    public double GetHeaderThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.TEE_HEADER_THICKNESS);
    public double GetMillTolerance() => Entity.GetDataReal(StartBaseRootFunctionKey.TEE_MILL_TOLERANCE);
    public double GetHeaderLength() => Entity.GetDataReal(StartBaseRootFunctionKey.TEE_HEADER_LENGTH);
    public double GetBranchHeight() => Entity.GetDataReal(StartBaseRootFunctionKey.TEE_BRANCH_HEIGHT);
    public double GetWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.TEE_WEIGHT);
    public double GetStrengthFactorOfLongitudinalWeldSeamOnPressure() => Entity.GetDataReal(StartBaseRootFunctionKey.TEE_STRENGTH_FACTOR_OF_LONGITUDINAL_WELD_SEAM_ON_PRESSURE);

    public override Dictionary<string, string> GetData()
    {
        var data = base.GetData();
        data.Add("Name", GetName());
        data.Add("Header Thickness", GetHeaderThickness().ToString("F"));
        data.Add("Mill Tolerance", GetMillTolerance().ToString("F"));
        data.Add("Header Length", GetHeaderLength().ToString("F"));
        data.Add("Branch Height", GetBranchHeight().ToString("F"));
        data.Add("Weight", GetWeight().ToString("F"));
        data.Add("Strength Factor Of Longitudinal Weld Seam On Pressure", GetStrengthFactorOfLongitudinalWeldSeamOnPressure().ToString("F"));
        
        return data;
    }
}