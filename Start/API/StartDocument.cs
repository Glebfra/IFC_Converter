using System.Reflection;
using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartDocument : IDisposable
{
    private object _document;

    public StartDocument(object document)
    {
        _document = document;
    }

    public StartBaseRootDataArray GetDataArrayDispatch()
    {
        object? dataArray = _document.GetType().InvokeMember(
            "GetDataArrayDispatch", BindingFlags.InvokeMethod, null, _document, null
        );
        return new StartBaseRootDataArray(dataArray);
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_document);
    }
}