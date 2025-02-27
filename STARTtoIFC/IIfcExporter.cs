using System;
using System.Runtime.InteropServices;

namespace STARTtoIFC
{
    [Guid("2649402A-A530-4CB8-A0C3-22661C463823")]
    public interface IIfcExporter
    {
        int Export(object startDocument, int languageId);

        int Test();
    }
}
