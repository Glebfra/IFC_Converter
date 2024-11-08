using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartDataSerialize : IDisposable
{
    private const string PROG_ID = "Data.CTAPTSerialize";
    
    private object _startDataSerialize;
    
    public StartDataSerialize()
    {
        Type? type = Type.GetTypeFromProgID(PROG_ID);
        if (type != null)
        {
            _startDataSerialize = Activator.CreateInstance(type);
        }
        else
        {
            throw new Exception($"Cannot find the prog_id: {PROG_ID}");
        }
    }

    public StartBaseRootDataArray GetDataArray(string filepath)
    {
        return new StartBaseRootDataArray(new object());
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_startDataSerialize);
    }
}