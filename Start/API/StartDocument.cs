using System.Reflection;
using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartDocument : IDisposable
{
    private const string PROG_ID = "CTAPT.Document";
    
    private object _document;

    public StartDocument()
    {
        Type? type = Type.GetTypeFromProgID(PROG_ID);
        if (type != null)
        {
            _document = Activator.CreateInstance(type);
        }
        else
        {
            throw new Exception($"Cannot find the prog_id: {PROG_ID}");
        }
    }

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

    public string GetTitle()
    {
        object? title = _document.GetType().InvokeMember(
            "GetTitle", BindingFlags.InvokeMethod, null, _document, null
        );
        return (string)title;
    }

    public string GetPathName()
    {
        object? pathName = _document.GetType().InvokeMember(
            "GetPathName", BindingFlags.InvokeMethod, null, _document, null
        );
        return (string)pathName;
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_document);
    }
}