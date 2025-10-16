using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class AVEVA_Pset : IPropertySet
    {
        public IfcLengthMeasure[] POS = new IfcLengthMeasure[3];
        public IfcReal[] ORI = new IfcReal[9];

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeBend";
                set.HasProperties.Add(model.Instances.New<IfcPropertyListValue>(value =>
                {
                    value.Name = nameof(POS);
                    value.ListValues.AddRange(POS.Cast<IfcValue>());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertyListValue>(value =>
                {
                    value.Name = nameof(ORI);
                    value.ListValues.AddRange(ORI.Cast<IfcValue>());
                }));
            });
        }
        
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

        public XbimVector3D GetPosition()
        {
            return new XbimVector3D(POS[0], POS[1], POS[2]);
        }

        public XbimMatrix3D GetObjectMatrix()
        {
            return new XbimMatrix3D(
                ORI[0], ORI[1], ORI[2], 0,
                -ORI[3], -ORI[4], -ORI[5], 0,
                ORI[6], ORI[7], ORI[8], 0,
                POS[0], POS[1], POS[2], 1
            );
        }
    }
}