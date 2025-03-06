using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Start.API
{
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

        public string GetTitle()
        {
            object? title = _document.GetType().InvokeMember(
                "GetTitle", BindingFlags.InvokeMethod, null, _document, null
            );
            return (string)title;
        }
        
        public string GetPathName()
        {
            object? title = _document.GetType().InvokeMember(
                "GetPathName", BindingFlags.InvokeMethod, null, _document, null
            );
            return (string)title;
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(_document);
        }
    }
}