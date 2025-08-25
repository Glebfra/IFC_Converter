using IFC.PropertySets;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCtoSTART.Extensions.PropertySets
{
    internal static class Pset_PipeFittingTypeJunctionExtensions
    {
        public static Pset_PipeFittingTypeJunction CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Pset_PipeFittingTypeJunction pset = new Pset_PipeFittingTypeJunction();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(pset.JunctionType):
                        pset.JunctionType = (IfcLabel)singleValue.NominalValue;
                        break;
                    case nameof(pset.JunctionLeftRadius):
                        pset.JunctionLeftRadius = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                        break;
                    case nameof(pset.JunctionLeftAngle):
                        pset.JunctionLeftAngle = (IfcPositivePlaneAngleMeasure)singleValue.NominalValue;
                        break;
                    case nameof(pset.JunctionRightRadius):
                        pset.JunctionRightRadius = (IfcPositiveLengthMeasure)singleValue.NominalValue;
                        break;
                    case nameof(pset.JunctionRightAngle):
                        pset.JunctionRightAngle = (IfcPositivePlaneAngleMeasure)singleValue.NominalValue;
                        break;
                }
            }

            return pset;
        }
    }
}