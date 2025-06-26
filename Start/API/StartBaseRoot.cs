using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Start.API
{
    public class StartBaseRoot : IDisposable
    {
        private readonly object _startBaseRoot;

        public int Id => GetNumber();

        public StartBaseRoot(object startBaseRoot)
        {
            _startBaseRoot = startBaseRoot;
        }

        public object GetObject()
        {
            return _startBaseRoot;
        }

        public int GetName()
        {
            object name = _startBaseRoot.GetType().InvokeMember(
                "GetName", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return (int)name;
        }

        public int GetNumberConn()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetNumberConn", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return (int)element;
        }

        public StartBaseRoot GetConnElemOnType(StartElementType type, int nNumber)
        {
            object element = new object();
            object[] args = { type, nNumber, element };

            ParameterModifier parameterModifier = new ParameterModifier(3) { [2] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            _startBaseRoot.GetType().InvokeMember(
                "GetConnElemOnType", BindingFlags.InvokeMethod, null, _startBaseRoot, args, modifiers, null, null
            );

            return new StartBaseRoot(args[2]);
        }

        public StartBaseRoot GetConnElemOnIndex(int nNumber)
        {
            object element = new object();
            object[] args = { nNumber, element };

            ParameterModifier parameterModifier = new ParameterModifier(2) { [1] = true };
            ParameterModifier[] modifiers = { parameterModifier };

            _startBaseRoot.GetType().InvokeMember(
                "GetConnElemOnIndex", BindingFlags.InvokeMethod, null, _startBaseRoot, args, modifiers, null, null
            );

            return new StartBaseRoot(args[1]);
        }

        public StartBaseRoot GetSNode()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetSNode", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return new StartBaseRoot(element);
        }

        public StartBaseRoot GetENode()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetENode", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return new StartBaseRoot(element);
        }
        
        public void SetSNode(int index)
        {
            object[] args = new object[] { index };
            _startBaseRoot.GetType().InvokeMember(
                "SetSNode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }
        
        public void SetENode(int index)
        {
            object[] args = new object[] { index };
            _startBaseRoot.GetType().InvokeMember(
                "SetENode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        public void SetSNode(StartBaseRoot node)
        {
            object[] args = new object[] { node._startBaseRoot };
            _startBaseRoot.GetType().InvokeMember(
                "SetSNode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }
        
        public void SetENode(StartBaseRoot node)
        {
            object[] args = new object[] { node._startBaseRoot };
            _startBaseRoot.GetType().InvokeMember(
                "SetENode", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        public string GetDataChar(int key)
        {
            object[] args = new object[] { key };
            object data = _startBaseRoot.GetType().InvokeMember(
                "GetDataChar", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
            return (string)data;
        }
        
        public int GetDataInt(int key)
        {
            object[] args = new object[] { key };
            object data = _startBaseRoot.GetType().InvokeMember(
                "GetDataInt", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
            return (int)data;
        }
        
        public double GetDataReal(int key)
        {
            object[] args = new object[] { key };
            object data = _startBaseRoot.GetType().InvokeMember(
                "GetDataReal", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
            return (double)data;
        }

        public int GetNumber()
        {
            object element = _startBaseRoot.GetType().InvokeMember(
                "GetNumber", BindingFlags.InvokeMethod, null, _startBaseRoot, null
            )!;
            return (int)element;
        }

        public double GetXCoord(int mode = 0)
        {
            object[] args = { mode };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetCoordX", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (double)value;
        }

        public double GetYCoord(int mode = 0)
        {
            object[] args = { mode };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetCoordY", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (double)value;
        }

        public double GetZCoord(int mode = 0)
        {
            object[] args = { mode };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetCoordZ", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (double)value;
        }

        public string GetDataJson(int mode = 0, int key = 0)
        {
            object[] args = { mode, key };
            object value = _startBaseRoot.GetType().InvokeMember(
                "GetDataJson", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            )!;
            return (string)value;
        }

        public void SetDataJson(int key, string data)
        {
            object[] args = { key, data };
            _startBaseRoot.GetType().InvokeMember(
                "SetDataJson", BindingFlags.InvokeMethod, null, _startBaseRoot, args
            );
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(_startBaseRoot);
        }
    }
}