namespace IFCConverter.Utils.IFC
{
    public class IfcExportType
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