using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.PropertySets
{
    public class AVEVA_Pset : IPropertySet
    {
        public IfcLengthMeasure[] POS = new IfcLengthMeasure[3];
        public IfcLengthMeasure[] ORI = new IfcLengthMeasure[9];

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
    }
}