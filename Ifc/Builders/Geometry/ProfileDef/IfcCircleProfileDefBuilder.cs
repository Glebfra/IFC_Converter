using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Geometry.ProfileDef
{
    public class IfcCircleProfileDefBuilder<T> : IfcParameterizedProfileDefBuilder<T>, IIfcCircleProfileDefBuilder<T>
        where T : IIfcCircleProfileDef, IInstantiableEntity
    {
        public IfcCircleProfileDefBuilder(double radius, IfcProfileTypeEnum profileTypeEnum, IfcLabel profileName)
            : base(profileTypeEnum, profileName)
        {
            Radius = radius;
        }

        public double Radius { get; }

        public override T CreateProfileDef(IModel model)
        {
            T profileDef = base.CreateProfileDef(model);
            profileDef.Radius = Radius;
            return profileDef;
        }
    }
}