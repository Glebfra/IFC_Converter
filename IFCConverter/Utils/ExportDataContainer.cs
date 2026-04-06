namespace IFCConverter.Utils
{
    internal class ExportDataContainer
    {
        public string InputFilePath { get; set; } = string.Empty;
        public string OutputFilePath { get; set; } = string.Empty;
        public int LanguageId { get; set; }
        public IfcExportTypeEnum ExportType { get; set; }
        public int NumSegments { get; set; }
    }
}