using IFC.Tools;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class AVEVA_EntityParameters : IPropertySet
    {
        public ActionProperty<IfcText> E3DType = new ActionProperty<IfcText>(string.Empty);
        public ActionProperty<IfcText> Name = new ActionProperty<IfcText>(string.Empty);
        public ActionProperty<IfcText> ObjectType = new ActionProperty<IfcText>(string.Empty);
        public ActionProperty<IfcText> Tag = new ActionProperty<IfcText>(string.Empty);

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = nameof(AVEVA_EntityParameters);
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(E3DType);
                    value.NominalValue = E3DType.Value;

                    E3DType.OnValueChange += () => value.NominalValue = E3DType.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(Name);
                    value.NominalValue = Name.Value;

                    Name.OnValueChange += () => value.NominalValue = Name.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(ObjectType);
                    value.NominalValue = ObjectType.Value;

                    ObjectType.OnValueChange += () => value.NominalValue = ObjectType.Value;
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = nameof(Tag);
                    value.NominalValue = Tag.Value;

                    Tag.OnValueChange += () => value.NominalValue = Tag.Value;
                }));
            });
        }
    }
}