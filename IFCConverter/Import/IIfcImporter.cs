using System;
using System.Runtime.InteropServices;

namespace IFCConverter
{
    [Obsolete("Use IfcConverter instead")]
    [Guid("1B568CE8-695E-4574-8AF1-3D9E2B3F9702")]
    public interface IIfcImporter
    {
        public int Import(object startDocumentObject, int languageId);

        public int Test();
    }
}