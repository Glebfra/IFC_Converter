namespace IFCConverter.Tools
{
    internal class ImportType
    {
        public ImportTypeEnum Type { get; set; }
        public string TypeName { get; set; }

        public ImportType(ImportTypeEnum type, string typeName)
        {
            Type = type;
            TypeName = typeName;
        }
    }
}