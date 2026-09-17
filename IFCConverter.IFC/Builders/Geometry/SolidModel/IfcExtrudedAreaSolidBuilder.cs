using IFCConverter.IFC.Extensions;
using IFCConverter.IFC.Interfaces.Geometry.SolidModel;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Builders.Geometry.SolidModel
{
    public class IfcExtrudedAreaSolidBuilder<T> : IfcSweptAreaSolidBuilder<T>, IIfcExtrudedAreaSolidBuilder<T>
        where T : IIfcExtrudedAreaSolid, IInstantiableEntity
    {
        public IfcExtrudedAreaSolidBuilder(double length, FixedVector<Dim3> extrusionDirection, IIfcProfileDef profileDef)
            : base(profileDef)
        {
            Length = length;
            ExtrusionDirection = extrusionDirection;
        }

        public FixedVector<Dim3> ExtrusionDirection { get; }
        public double Length { get; }

        public override T CreateSolidModel(IModel model)
        {
            T solid = base.CreateSolidModel(model);
            solid.Depth = Length;
            solid.ExtrudedDirection = ExtrusionDirection.ToIfcDirection(model);
            return solid;
        }
    }
}