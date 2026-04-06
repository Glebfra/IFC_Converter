using Ifc.Extensions;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.SolidModel
{
    public class IfcExtrudedAreaSolidBuilder<T> : IfcSweptAreaSolidBuilder<T>, IIfcExtrudedAreaSolidBuilder<T>
        where T : IIfcExtrudedAreaSolid, IInstantiableEntity
    {
        public IfcExtrudedAreaSolidBuilder(double length, Vector<double> extrusionDirection, IIfcProfileDef profileDef)
            : base(profileDef)
        {
            Length = length;
            ExtrusionDirection = extrusionDirection;
        }

        public Vector<double> ExtrusionDirection { get; }
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