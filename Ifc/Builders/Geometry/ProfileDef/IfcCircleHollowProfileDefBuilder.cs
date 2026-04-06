using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Geometry.ProfileDef
{
    public class IfcCircleHollowProfileDefBuilder<T> : IfcCircleProfileDefBuilder<T>,
        IIfcCircleHollowProfileDefBuilder<T>
        where T : IIfcCircleHollowProfileDef, IInstantiableEntity
    {
        public IfcCircleHollowProfileDefBuilder(double radius, double wallThickness, IfcProfileTypeEnum profileTypeEnum,
            IfcLabel profileName)
            : base(radius, profileTypeEnum, profileName)
        {
            WallThickness = wallThickness;
        }

        public double WallThickness { get; }

        public override T CreateProfileDef(IModel model)
        {
            T profileDef = base.CreateProfileDef(model);
            profileDef.WallThickness = WallThickness;
            return profileDef;
        }
    }
}