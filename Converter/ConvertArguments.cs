namespace Converter
{
    public class ConvertArguments
    {
        public const string ConvertTypeArgument = "-T";
        public const string CtpFilePathArgument = "-C";
        public const string IfcFilePathArgument = "-I";

        public ConvertTypeEnum? ConvertType { get; set; }
        public string? CtpFilePath { get; set; }
        public string? IfcFilePath { get; set; }
    }
}