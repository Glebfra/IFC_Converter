using System.Reflection;
using System.Runtime.InteropServices;

namespace IFC_Converter.Start;

public class StartApiWrapper : IDisposable
{
    private static object _autoServer;
    private static object _document;
    private static object _dataArray;

    public static StartApiWrapper Create(string fileName)
    {
        try
        {
            InitAutoServer();
            _autoServer.GetType().InvokeMember("Visible", BindingFlags.SetProperty, null, _autoServer,
                new object[] { true });
            _autoServer.GetType().InvokeMember("SaveToFile", BindingFlags.InvokeMethod, null, _autoServer,
                new object[] { fileName });
            InitDocument(0x2, fileName);
            InitDataArray();

            //delete the first node (by default the new START-PROF file contain one node)
            _dataArray.GetType().InvokeMember("DeleteElement", BindingFlags.InvokeMethod, null, _dataArray,
                new object[] { 0 });

            //Access to the main window
            _autoServer.GetType().InvokeMember("EnableMainWindow", BindingFlags.InvokeMethod, null, _autoServer,
                new object[] { 0 });

            return new StartApiWrapper();
        }
        catch (Exception ex)
        {
            throw new Exception("Error launching START-PROF", ex);
        }
    }

    private static string GetFullName()
    {
        InitAutoServer();
        object fullName = _autoServer.GetType().InvokeMember("FullName", BindingFlags.InvokeMethod, null, _autoServer, 
            new object[] { });
        return (string)fullName;
    }

    public static void InitAutoServer()
    {
        string progID = "CTAPT.AutoServer";
        Type type = Type.GetTypeFromProgID(progID);
        _autoServer = Activator.CreateInstance(type);
    }

    private static void InitDocument(int mode, string fileName)
    {
        _document = _autoServer.GetType().InvokeMember("LoadCTAPTDocument", BindingFlags.InvokeMethod, 
            null, _autoServer, new object[] { mode, fileName, 0 });
    }

    private static void InitDataArray()
    {
        _dataArray = _document.GetType().InvokeMember("GetDataArrayDispatch", BindingFlags.InvokeMethod, 
            null, _document, null);
    }

    public void Finish()
    {
        _document.GetType().InvokeMember("GetPipeline", BindingFlags.InvokeMethod, null, _document, null);

        object[] args = new object[] { 1, 3 };
        _document.GetType().InvokeMember("SetViewOfModel", BindingFlags.InvokeMethod, null, _document, args);

        _autoServer.GetType().InvokeMember("EnableMainWindow", BindingFlags.InvokeMethod, null, _autoServer,
            new object[] { 1 });

        _document.GetType().InvokeMember("DrawViewAll", BindingFlags.InvokeMethod, null, _document, null);
    }

    public object AddElement(int type)
    {
        ParameterModifier p3 = new ParameterModifier(2);
        p3[1] = true;
        ParameterModifier[] mods3 = { p3 };

        object pElem = new object();
        object[] args = new object[] { type, pElem };

        _dataArray.GetType().InvokeMember("AddElement", BindingFlags.InvokeMethod, null, _dataArray, args, mods3, null,
            null);
        return args[1];
    }

    public void ConnectElementWithNode(object element, int nodeInternalId)
    {
        element.GetType().InvokeMember("SetConnElem", BindingFlags.InvokeMethod, null, element,
            new object[] { nodeInternalId });
    }

    internal int GetNumber(object element)
    {
        return (int)element.GetType().InvokeMember("GetNumber", BindingFlags.InvokeMethod, null, element, null);
    }

    public object SetName(object node, int nodeId)
    {
        return node.GetType().InvokeMember("SetName", BindingFlags.InvokeMethod, null, node, new object[] { nodeId });
    }

    public void SetBeginNode(object element, int indexNode)
    {
        element.GetType()
            .InvokeMember("SetSNode", BindingFlags.InvokeMethod, null, element, new object[] { indexNode });
    }

    public void SetEndNode(object element, int indexNode)
    {
        element.GetType()
            .InvokeMember("SetENode", BindingFlags.InvokeMethod, null, element, new object[] { indexNode });
    }

    public void SetDataReal(object element, int attribute, double value)
    {
        element.GetType().InvokeMember("SetDataReal", BindingFlags.InvokeMethod, null, element,
            new object[] { attribute, value });
    }

    public void SetDataInt(object element, int attribute, int value)
    {
        element.GetType().InvokeMember("SetDataInt", BindingFlags.InvokeMethod, null, element,
            new object[] { attribute, value });
    }

    public void SetDataChar(object element, int attribute, string value)
    {
        element.GetType().InvokeMember("SetDataChar", BindingFlags.InvokeMethod, null, element,
            new object[] { attribute, value });
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_dataArray);
        Marshal.ReleaseComObject(_document);
        Marshal.ReleaseComObject(_autoServer);
    }
}