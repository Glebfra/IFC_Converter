using System;
using System.Linq;
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

        public void SetViewOfModel(int[] view)
        {
            object[] args = view.Cast<object>().ToArray();
            _document.GetType().InvokeMember("SetViewOfModel", BindingFlags.InvokeMethod, null, _document, args);
        }

        public void DrawViewAll()
        {
            _document.GetType().InvokeMember("DrawViewAll", BindingFlags.InvokeMethod, null, _document, null);
        }

        public void DrawFitAll()
        {
            _document.GetType().InvokeMember("DrawFitAll", BindingFlags.InvokeMethod, null, _document, null);
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
            if (_document != null)
                Marshal.ReleaseComObject(_document);
        }
    }
}