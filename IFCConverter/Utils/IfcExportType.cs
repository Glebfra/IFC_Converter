namespace IFCConverter.Utils
{
    internal class IfcExportType
    {
        public IfcExportType(IfcExportTypeEnum type, string typeName)
        {
            Type = type;
            TypeName = typeName;
        }

        public IfcExportTypeEnum Type { get; set; }
        public string TypeName { get; set; }
    }
}