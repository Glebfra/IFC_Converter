using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Geometry.ProfileDef
{
    public class IfcProfileDefBuilder<T> : IIfcProfileDefBuilder<T>
        where T : IIfcProfileDef, IInstantiableEntity
    {
        public IfcProfileDefBuilder(IfcProfileTypeEnum profileTypeEnum, IfcLabel profileName)
        {
            ProfileTypeEnum = profileTypeEnum;
            ProfileName = profileName;
        }

        public object? Instance => ProfileDef;

        public T? ProfileDef { get; private set; }

        public IfcProfileTypeEnum ProfileTypeEnum { get; }
        public IfcLabel ProfileName { get; }

        public virtual T CreateProfileDef(IModel model)
        {
            ProfileDef = model.Instances.New<T>(def =>
            {
                def.ProfileName = ProfileName;
                def.ProfileType = ProfileTypeEnum;
            });
            return ProfileDef;
        }

        public object Build(IModel model)
        {
            return CreateProfileDef(model);
        }
    }
}