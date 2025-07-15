using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class AVEVA_EntityParameters
    {
        public string E3DType = string.Empty;
        public string Name = string.Empty;
        public string ObjectType = string.Empty;
        public string Tag = string.Empty;
        
        public AVEVA_EntityParameters() {}

        public static AVEVA_EntityParameters CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            AVEVA_EntityParameters pset = new AVEVA_EntityParameters();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(pset.E3DType):
                        pset.E3DType = (IfcText)singleValue.NominalValue;
                        break;
                    case nameof(pset.Name):
                        pset.Name = (IfcText)singleValue.NominalValue;
                        break;
                    case nameof(pset.ObjectType):
                        pset.ObjectType = (IfcText)singleValue.NominalValue;
                        break;
                    case nameof(pset.Tag):
                        pset.Tag = (IfcText)singleValue.NominalValue;
                        break;
                }
            }

            return pset;
        }
    }
}