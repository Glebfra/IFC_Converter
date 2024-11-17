using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public class StartWeldingTeeEntity : StartAbstractEntity
{
    public StartWeldingTeeEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public StartNodeEntity GetConnNode()
    {
        return new StartNodeEntity(entity.GetConnElemOnType(StartElementType.PIPE_ELEMENT, 0));
    }

    public string GetName()
    {
        return entity.GetDataChar(StartBaseRootFunctionKey.WELDED_TEE_NAME);
    }

    public double GetHeaderThickness()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_HEADER_THICKNESS);
    }
    
    public double GetMillTolerance()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_MILL_TOLERANCE);
    }
    
    public double GetWeight()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_WEIGHT);
    }
    
    public double GetHeaderLength()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_HEADER_LENGTH);
    }
    
    public double GetStrengthFactorOfLongitudinalWeldSeamOnPressure()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_STRENGTH_FACTOR_OF_LONGITUDINAL_WELD_SEAM_ON_PRESSURE);
    }
    
    public double GetBranchHeight()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_BRANCH_HEIGHT);
    }
    
    public double GetCrotchRadius()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_RADIUS);
    }
    
    public double GetCrotchThickness()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_THICKNESS);
    }
    
    public double GetCrotchHeight()
    {
        return entity.GetDataReal(StartBaseRootFunctionKey.WELDED_TEE_CROTCH_HEIGHT);
    }

    public override Dictionary<string, string> GetData()
    {
        var dictionary = base.GetData();
        dictionary.Add("Header thickness", GetHeaderThickness().ToString());
        dictionary.Add("Mill tolerance", GetMillTolerance().ToString());
        dictionary.Add("Weight", GetWeight().ToString());
        dictionary.Add("Header length", GetHeaderLength().ToString());
        dictionary.Add("Strength factor of longitudinal weld seam on pressure", GetStrengthFactorOfLongitudinalWeldSeamOnPressure().ToString());
        dictionary.Add("Branch height", GetBranchHeight().ToString());
        dictionary.Add("Crotch radius", GetCrotchRadius().ToString());
        dictionary.Add("Crotch thickness", GetCrotchThickness().ToString());
        dictionary.Add("Crotch height", GetCrotchHeight().ToString());
        dictionary.Add("Name", GetName());

        return dictionary;
    }
}