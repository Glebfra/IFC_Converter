using System.Reflection;
using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartAutoServer : IDisposable
{
    private const string PROG_ID = "CTAPT.AutoServer";
    
    private readonly object _autoServer;
    
    public StartAutoServer()
    {
        Type type = Type.GetTypeFromProgID(PROG_ID);
        _autoServer = Activator.CreateInstance(type);
    }

    public string GetFullName()
    {
        object fullName = _autoServer.GetType().InvokeMember(
            "FullName", BindingFlags.InvokeMethod, null, _autoServer, null
        );
        return (string)fullName;
    }

    public string GetLastError()
    {
        object lastError = _autoServer.GetType().InvokeMember(
            "LastError", BindingFlags.InvokeMethod, null, _autoServer, null
        );
        return (string)lastError;
    }

    public StartDocument LoadFile(string fileName)
    {
        object document = _autoServer.GetType().InvokeMember(
            "LoadFile", BindingFlags.InvokeMethod, null, _autoServer, new object[] { fileName }
        );
        return new StartDocument(document);
    }

    public BaseRootDataArray GetDataArray()
    {
        object dataArray = _autoServer.GetType().InvokeMember(
            "GetDataArray", BindingFlags.InvokeMethod, null, _autoServer, null
        );
        return new BaseRootDataArray(dataArray);
    }

    public object GetDataArrayDispatch()
    {
        object dataArray = _autoServer.GetType().InvokeMember(
            "GetDataArrayDispatch", BindingFlags.InvokeMethod, null, _autoServer, null
        );
        return dataArray;
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_autoServer);
    }
}