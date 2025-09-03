using IFC.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace STARTtoIFC.Extensions.PropertySets
{
    internal class AVEVA_EntityParametersExtensions
    {
        public static AVEVA_EntityParameters CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            AVEVA_EntityParameters pset = new AVEVA_EntityParameters();
            
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(pset.Name):
                        pset.Name.Value = (IfcText)singleValue.NominalValue;
                        break;
                    case nameof(pset.Tag):
                        pset.Tag.Value = (IfcText)singleValue.NominalValue;
                        break;
                    case nameof(pset.ObjectType):
                        pset.ObjectType.Value = (IfcText)singleValue.NominalValue;
                        break;
                    case nameof(pset.E3DType):
                        pset.E3DType.Value = (IfcText)singleValue.NominalValue;
                        break;
                }
            }

            return pset;
        }
    }
}