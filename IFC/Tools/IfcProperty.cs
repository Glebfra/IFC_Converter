using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC_Converter.IFC.Tools;

public static class IfcProperty
{
    public static IfcRelDefinesByProperties AddProperties(IModel model, IfcObject ifcObject, StartAbstractEntity entity)
    {
        return model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(ifcObject);
            rel.RelatingPropertyDefinition = CreatePropertySet(model, "Properties", entity);
        });
    }

    public static IfcPropertySet CreatePropertySet(IModel model, string name, StartAbstractEntity entity)
    {
        return model.Instances.New<IfcPropertySet>(set =>
        {
            set.Name = name;
            foreach (var kvp in entity.GetData())
            {
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(prop =>
                {
                    prop.Name = kvp.Key;
                    prop.NominalValue = new IfcText(kvp.Value);
                }));
            }
        });
    }
}