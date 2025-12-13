using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Start.API
{
    public class StartAutoServer : IDisposable
    {
        public object AutoServer => _autoServer;
        
        private const string PROG_ID = "CTAPT.AutoServer";

        private readonly object _autoServer;

        public StartAutoServer()
        {
            Type? type = Type.GetTypeFromProgID(PROG_ID);
            if (type != null)
            {
                _autoServer = Activator.CreateInstance(type);
            }
            else
            {
                throw new Exception($"Cannot find the prog_id: {PROG_ID}");
            }
        }
        
        public StartAutoServer(object autoServer)
        {
            _autoServer = autoServer ?? throw new ArgumentNullException(nameof(autoServer));
        }

        public StartDocument LoadStartDocument(int mode, string filepath)
        {
            object document = LoadStartDocumentRaw(mode, filepath);
            return new StartDocument(document);
        }
        
        public object LoadStartDocumentRaw(int mode, string filepath)
        {
            object? document = _autoServer.GetType().InvokeMember(
                "LoadCTAPTDocument", BindingFlags.InvokeMethod, null, _autoServer, new object[] { mode, filepath, 0 }
            );
            if (document == null)
                throw new Exception("Failed to load Start document.");
            return document;
        }

        public void SaveToFile(string filepath)
        {
            object[] args = new object[] { filepath };
            _autoServer.GetType().InvokeMember("SaveToFile", BindingFlags.InvokeMethod, null, _autoServer, args);
        }

        public string? GetFullName()
        {
            object? fullName = _autoServer.GetType().InvokeMember(
                "FullName", BindingFlags.InvokeMethod, null, _autoServer, null
            );
            return (string?)fullName;
        }

        public StartBaseRootDataArray GetDataArrayDispatch()
        {
            object? dataArray = _autoServer.GetType().InvokeMember(
                "GetDataArrayDispatch", BindingFlags.InvokeMethod, null, _autoServer, null
            );
            return new StartBaseRootDataArray(dataArray);
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(_autoServer);
        }
    }
}