using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Start.API
{
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

        public StartBaseRoot GetConnDispatch(int id, int nNumber)
        {
            object element = new object();
            object[] args = { id, nNumber, element };

            ParameterModifier parameterModifier = new ParameterModifier(3) { [2] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            _startBaseRootDataArray.GetType().InvokeMember(
                "GetConnDispatch", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args, modifiers, null, null
            );

            return new StartBaseRoot(args[2]);
        }

        public int GetNumberElements(StartElementType minType = StartElementType.ALL,
            StartElementType maxType = StartElementType.ALL)
        {
            object[] args = { minType, maxType };
            object? elementsNumber = _startBaseRootDataArray.GetType().InvokeMember(
                "GetNumberElements", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );

            return (int)elementsNumber;
        }

        public int GetNumberConns(int id, StartElementType minType, StartElementType maxType)
        {
            object[] args = { id, minType, maxType };
            object? elementsNumber = _startBaseRootDataArray.GetType().InvokeMember(
                "GetNumberConns", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );
            return (int)elementsNumber;
        }

        public string GetTitle(int id)
        {
            object[] args = { id };
            object? title = _startBaseRootDataArray.GetType().InvokeMember(
                "GetTitle", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );

            return (string)title;
        }

        public string GetDataJson(StartElementType minType, StartElementType maxType, int mode = 0)
        {
            object[] args = { mode, minType, maxType };
            object? dataJson = _startBaseRootDataArray.GetType().InvokeMember(
                "GetInputDataJsonArray", BindingFlags.InvokeMethod, null, _startBaseRootDataArray, args
            );
            
            return (string)dataJson ?? "{}";
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(_startBaseRootDataArray);
        }
    }
}