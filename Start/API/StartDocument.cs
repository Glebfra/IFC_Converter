using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartDocument : IDisposable
{
    private object _document;

    public StartDocument(object document)
    {
        _document = document;
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_document);
    }
}