using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces.Geometry.ProfileDef
{
    public interface IIfcProfileDefBuilder<out T> : IIfcBuilder
        where T : IIfcProfileDef
    {
        T ProfileDef { get; }

        IfcProfileTypeEnum ProfileTypeEnum { get; }
        IfcLabel ProfileName { get; }

        T CreateProfileDef(IModel model);
    }
}