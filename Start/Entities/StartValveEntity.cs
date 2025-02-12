using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities;

public class StartValveEntity : StartAbstractEntity
{
    public StartValveEntity(StartBaseRoot entity) : base(entity)
    {
    }

    public string GetName() => Entity.GetDataChar(StartBaseRootFunctionKey.VALVE_NAME);
    public double GetOutsideDiameter() => Entity.GetDataReal(StartBaseRootFunctionKey.VALVE_OUTSIDE_DIAMETER);
    public double GetWeight() => Entity.GetDataReal(StartBaseRootFunctionKey.VALVE_WEIGHT);
    public double GetLength() => Entity.GetDataReal(StartBaseRootFunctionKey.VALVE_LENGTH);
    public int GetLeakageCheck() => Entity.GetDataInt(StartBaseRootFunctionKey.VALVE_LEAKAGE_CHECK);
    public double GetGasketEffectiveDiameter() => Entity.GetDataReal(StartBaseRootFunctionKey.VALVE_GASKET_EFFECTIVE_DIAMETER);
    public double GetNominalPressure() => Entity.GetDataReal(StartBaseRootFunctionKey.VALVE_NOMINAL_PRESSURE);
    public int GetGasketCrossection() => Entity.GetDataInt(StartBaseRootFunctionKey.VALVE_GASKET_CROSSECTION);

    public override Dictionary<string, string> GetData()
    {
        Dictionary<string, string> data = base.GetData();
        data.Add("Name", GetName());
        data.Add("Outside Diameter", GetOutsideDiameter().ToString("F5"));
        data.Add("Weight", GetWeight().ToString("F5"));
        data.Add("Length", GetLength().ToString("F5"));
        data.Add("Leakage Check", GetLeakageCheck().ToString());
        data.Add("Gasket Effective Diameter", GetGasketEffectiveDiameter().ToString("F5"));
        data.Add("Nominal Pressure", GetNominalPressure().ToString("F5"));
        data.Add("Gasket Crossection", GetGasketCrossection().ToString());

        return data;
    }
}