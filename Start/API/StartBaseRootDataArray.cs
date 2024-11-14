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

    public StartBaseRoot GetElementDispatch(int id, StartElementType minType, StartElementType maxType)
    {
        object element = new object();
        object[] args = { id, minType, maxType, element };
        
        ParameterModifier parameterModifier = new ParameterModifier(4) { [3] = true };
        ParameterModifier[] modifiers = { parameterModifier };
        
        _startBaseRootDataArray.GetType().InvokeMember(
            "GetElementDispatch", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null, null
        );
        
        return new StartBaseRoot(args[3]);
    }

    public object GetConnDispatch(int id, int nNumber)
    {
        object element = new object();
        object[] args = { id, nNumber, element };

        ParameterModifier parameterModifier = new ParameterModifier(3) { [2] = true };
        ParameterModifier[] modifiers = { parameterModifier };

        _startBaseRootDataArray.GetType().InvokeMember(
            "GetConnDispatch", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null, null
        );
        
        return args[2];
    }

    public int GetNumberElements(StartElementType minType = StartElementType.ALL, StartElementType maxType = StartElementType.ALL)
    {
        object? elementsNumber = _startBaseRootDataArray.GetType().InvokeMember(
            "GetNumberElements", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, new object[]{minType, maxType}
        );
        
        return (int)elementsNumber;
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