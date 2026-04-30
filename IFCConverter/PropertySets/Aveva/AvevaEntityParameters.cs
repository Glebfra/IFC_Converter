using IFCConverter.Attributes;

namespace IFCConverter.PropertySets.Aveva
{
    [PropertySet(name: "AVEVA_EntityParameters")]
    internal class AvevaEntityParameters : AbstractPropertySet
    {
        [Property("E3DType")] public string E3DType;
        [Property("Name")] public string Name;
        [Property("ObjectType")] public string ObjectType;
        [Property("Tag")] public string Tag;
    }
}