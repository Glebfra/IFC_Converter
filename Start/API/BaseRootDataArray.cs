using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class BaseRootDataArray : IDisposable
{
    private object _startBaseRootDataArray;

    public BaseRootDataArray(object startBaseRootDataArray)
    {
        _startBaseRootDataArray = startBaseRootDataArray;
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_startBaseRootDataArray);
    }
}