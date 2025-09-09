using System.Linq;
using IFC.PropertySets;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Extensions.PropertySets
{
    internal static class AVEVA_PsetExtensions
    {
        public static AVEVA_Pset CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            AVEVA_Pset pset = new AVEVA_Pset();
            
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IIfcPropertyListValue? propertyListValue = property as IIfcPropertyListValue;
                if (propertyListValue != null)
                {
                    switch (property.Name)
                    {
                        case nameof(pset.POS):
                            pset.POS = propertyListValue.ListValues.Cast<IfcLengthMeasure>().ToArray();
                            break;
                        case nameof(pset.ORI):
                            pset.ORI = propertyListValue.ListValues.Cast<IfcReal>().ToArray();
                            break;
                    }
                }
            }

            return pset;
        }

        public static XbimVector3D GetPosition(this AVEVA_Pset avevaPset)
        {
            return new XbimVector3D(
                avevaPset.POS[0],
                avevaPset.POS[1],
                avevaPset.POS[2]
            );
        }

        public static XbimMatrix3D GetObjectMatrix(this AVEVA_Pset avevaPset)
        {
            return new XbimMatrix3D(
                avevaPset.ORI[0], avevaPset.ORI[1], avevaPset.ORI[2], 0,
                avevaPset.ORI[3], avevaPset.ORI[4], avevaPset.ORI[5], 0,
                avevaPset.ORI[6], avevaPset.ORI[7], avevaPset.ORI[8], 0,
                avevaPset.POS[0], avevaPset.POS[1], avevaPset.POS[2], 1
            );
        }
    }
}