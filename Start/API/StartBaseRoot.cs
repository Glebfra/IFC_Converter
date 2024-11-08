using System.Reflection;
using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartBaseRoot : IDisposable
{
    private readonly object _startBaseRoot;

    public StartBaseRoot(object startBaseRoot)
    {
        _startBaseRoot = startBaseRoot;
    }

    public int GetDataInt(StartBaseRootFunctionKey key)
    {
        object[] args = { key };
        object value = _startBaseRoot.GetType().InvokeMember(
            "GetDataInt", BindingFlags.InvokeMethod, null, _startBaseRoot, args
        );
        return (int)value;
    }

    public double GetDataReal(StartBaseRootFunctionKey key)
    {
        object[] args = { key };
        object value = _startBaseRoot.GetType().InvokeMember(
            "GetDataReal", BindingFlags.InvokeMethod, null, _startBaseRoot, args
        );
        return (double)value;
    }

    public string GetDataChar(StartBaseRootFunctionKey key)
    {
        object[] args = { key };
        object value = _startBaseRoot.GetType().InvokeMember(
            "GetDataChar", BindingFlags.InvokeMethod, null, _startBaseRoot, args
        );
        return (string)value;
    }
    
    public void Dispose()
    {
        Marshal.ReleaseComObject(_startBaseRoot);
    }
}