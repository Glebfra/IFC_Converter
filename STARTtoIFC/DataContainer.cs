namespace STARTtoIFC
{
    internal class DataContainer
    {
        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public int LanguageId { get; set; }
        public IfcExportTypeEnum ExportType { get; set; }
    }
}