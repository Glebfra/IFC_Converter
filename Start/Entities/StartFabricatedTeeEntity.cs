using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public class StartFabricatedTeeEntity : StartAbstractTeeEntity
{
    public StartFabricatedTeeEntity(StartBaseRoot entity) : base(entity)
    {
    }
    
    public double GetBranchWallThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_BRANCH_WALL_THICKNESS);
    public double GetMillToleranceForBranch() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_MILL_TOLERANCE_FOR_BRANCH);
    public double GetPadThickness() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_PAD_THICKNESS);
    public double GetPadWidth() => Entity.GetDataReal(StartBaseRootFunctionKey.FABRICATED_TEE_PAD_WIDTH);

    public override Dictionary<string, string> GetData()
    {
        var data = base.GetData();
        data.Add("Branch Wall Thickness", GetBranchWallThickness().ToString("F5"));
        data.Add("Mill Tolerance For Branch", GetMillToleranceForBranch().ToString("F5"));
        data.Add("Pad Thickness", GetPadThickness().ToString("F5"));
        data.Add("Pad Width", GetPadWidth().ToString("F5"));

        return data;
    }
}