using System;
using Start.API;

namespace STARTtoIFC
{
    internal static class EventBus
    {
        public static Action<StartDocument, string>? OnExport;
        public static Action<ConversionResult>? OnExportFinished;
    }
}