using IFC.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCtoSTART.Extensions.PropertySets
{
    internal static class Pset_PipeFittingTypeBendExtensions
    {
        public static Pset_PipeFittingTypeBend CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Pset_PipeFittingTypeBend pset = new Pset_PipeFittingTypeBend();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(pset.BendAngle):
                        pset.BendAngle = (IfcPositivePlaneAngleMeasure)singleValue.NominalValue;
                        break;
                    case nameof(pset.BendRadius):
                        pset.BendRadius = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                        break;
                }
            }

            return pset;
        }
    }
}