using IFCConverter.Importer.Attributes;

namespace IFCConverter.Importer.PropertySets.Aveva
{
    #pragma warning disable CS0414
    [PropertySet(name: "AVEVA_EntityParameters")]
    internal class AvevaEntityParameters : AbstractPropertySet
    {
        [Property("E3DType")] public string E3DType = string.Empty;
        [Property("Name")] public string Name = string.Empty;
        [Property("ObjectType")] public string ObjectType = string.Empty;
        [Property("Tag")] public string Tag = string.Empty;
    }
    #pragma warning restore CS0414
}