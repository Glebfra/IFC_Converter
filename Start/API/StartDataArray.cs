using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartDataArray : IDisposable
{
    private object _startDataArray;
    
    public void Dispose()
    {
        Marshal.ReleaseComObject(_startDataArray);
    }
}