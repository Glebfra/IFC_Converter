using System.Reflection;
using System.Runtime.InteropServices;

namespace IFC_Converter.Start.API;

public class StartBaseRootDataArray : IDisposable
{
    private readonly object _startBaseRootDataArray;

    public StartBaseRootDataArray(object startBaseRootDataArray)
    {
        _startBaseRootDataArray = startBaseRootDataArray;
    }

    public object GetElementDispatch(int id, StartElementType minType, StartElementType maxType)
    {
        object element = new object();
        object[] args = { id, minType, maxType, element };
        _startBaseRootDataArray.GetType().InvokeMember(
            "GetElementDispatch", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
        );
        return args[3];
    }

    public int GetNumberElements(StartElementType minType = StartElementType.ALL, StartElementType maxType = StartElementType.ALL)
    {
        object? elementsNumber = _startBaseRootDataArray.GetType().InvokeMember(
            "GetNumberElements", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, new object[]{minType, maxType}
        );
        return (int)elementsNumber;
    }

    public int GetSelectedElement()
    {
        int id = 0;
        StartElementType type = StartElementType.ALL;
        object[] args = { id, type };
        object newId = _startBaseRootDataArray.GetType().InvokeMember(
            "GetSelectedElement", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
        );
        return (int)newId;
    }

    public string GetTitle(int id)
    {
        object? title = _startBaseRootDataArray.GetType().InvokeMember(
            "GetTitle", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, new object[] { id }
        );
        return (string)title;
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_startBaseRootDataArray);
    }
}