using Start.API;

namespace Start.Entities.Abstract;

public class StartAbstractReducerEntity : StartAbstractEntity
{
    public StartAbstractReducerEntity(StartBaseRoot entity) : base(entity)
    {
    }
    
    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.REDUCER_NAME);
    public double GetWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_WEIGHT);
    public int GetManufacturingTechnology() => Entity.GetDataInt(StartBaseRootFunctionKey.REDUCER_MANUFACTURING_TECHNOLOGY);
    public double GetMillToleranceAtDMax() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_MILL_TOLERANCE_AT_D_MAX);
    public double GetMillToleranceAtDMin() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_MILL_TOLERANCE_AT_D_MIN);
    public double GetMillTolerance() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_MILL_TOLERANCE);
    public double GetLengthOfConicalPart() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_LENGTH_OF_CONICAL_PART);
    public double GetMaxDiameter() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_MAX_DIAMETER);
    public double GetMinDiameter() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_MIN_DIAMETER);
    public double GetThicknessAtMaxDiameterPoint() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_THICKNESS_AT_MAX_DIAMETER_POINT);
    public double GetAngleBetweenEccentricityVectorAndZmAxis() => Entity.GetDataReal(StartBaseRootFunctionKey.REDUCER_ANGLE_BETWEEN_ECCENTRICITY_VECTOR_AND_ZM_AXIS);

    public override Dictionary<string, string> GetData()
    {
        var data = base.GetData();
        
        data.Add("Name", GetName());
        data.Add("Weight", GetWeight().ToString("F"));
        data.Add("Manufacturing Technology", GetManufacturingTechnology().ToString());
        data.Add("Mill Tolerance At D Max", GetMillToleranceAtDMax().ToString("F"));
        data.Add("Mill Tolerance At D Min", GetMillToleranceAtDMin().ToString("F"));
        data.Add("Mill Tolerance", GetMillTolerance().ToString("F"));
        data.Add("Length Of Conical Part", GetLengthOfConicalPart().ToString("F"));
        data.Add("Max Diameter", GetMaxDiameter().ToString("F"));
        data.Add("Min Diameter", GetMinDiameter().ToString("F"));
        data.Add("Thickness At Max Diameter Point", GetThicknessAtMaxDiameterPoint().ToString("F"));
        data.Add("Angle Between Eccentricity Vector And Zm Axis", GetAngleBetweenEccentricityVectorAndZmAxis().ToString("F"));

        return data;
    }
}