namespace STARTtoIFC
{
    internal class IfcExportType
    {
        public IfcExportTypeEnum Type { get; set; }
        public string TypeName { get; set; }

        public IfcExportType(IfcExportTypeEnum type, string typeName)
        {
            Type = type;
            TypeName = typeName;
        }
    }
}