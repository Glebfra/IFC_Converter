using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    internal class Pset_Debug : IPropertySet
    {
        public ActionProperty<XbimMatrix3D> ObjectMatrix3D;

        public Pset_Debug(ActionProperty<XbimMatrix3D> objectMatrix3D)
        {
            ObjectMatrix3D = objectMatrix3D;
        }

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = nameof(Pset_Debug);
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Coordinates";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Value.Translation.ToString());
                    
                    ObjectMatrix3D.OnValueChange += () => value.NominalValue = new IfcText(ObjectMatrix3D.Value.Translation.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Forward direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Value.Forward.ToString());
                    
                    ObjectMatrix3D.OnValueChange += () => value.NominalValue = new IfcText(ObjectMatrix3D.Value.Forward.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Right direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Value.Right.ToString());
                    
                    ObjectMatrix3D.OnValueChange += () => value.NominalValue = new IfcText(ObjectMatrix3D.Value.Right.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Up direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Value.Up.ToString());
                    
                    ObjectMatrix3D.OnValueChange += () => value.NominalValue = new IfcText(ObjectMatrix3D.Value.Up.ToString());
                }));
            });
        }
    }
}