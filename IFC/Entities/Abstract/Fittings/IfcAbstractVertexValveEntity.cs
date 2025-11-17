using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Interfaces;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexValveEntity : IfcAbstractFittingEntity
    {
        public abstract ActionProperty<double> Diameter { get; }
        public abstract ActionProperty<double> Angle { get; }
        public abstract ActionProperty<int> NumSegments { get; }

        protected IfcAbstractVertexValveEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcValve pipeFitting = CreateIfcEntity<IfcValve>(model);
            ClipPipes();
            return pipeFitting;
        }
        
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcValve, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcValveTypeEnum.NOTDEFINED;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimVector3D displacement = Length / 2 * VectorExtensions.Forward;

            IfcCartesianPoint[] firstCircle = IfcGeometry.CreateCircle(model, Diameter * 0.75, displacement.Negated(), NumSegments);
            IfcCartesianPoint[] secondCircle = IfcGeometry.CreateCircle(model, Diameter * 0.75, displacement, NumSegments);
            foreach (IfcCartesianPoint secondCirclePoint in secondCircle)
                secondCirclePoint.RotateAroundYAxis(Angle);

            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
            IfcFacetedBrep lowerBrep = IfcGeometry.CreateCone(model, firstCircle, topPoint);
            IfcFacetedBrep upperBrep = IfcGeometry.CreateCone(model, secondCircle, topPoint);

            return new IfcRepresentationItem[] { lowerBrep, upperBrep };
        }

        private void ClipPipes()
        {
            IEnumerable<IIfcClippable> clippables = ConnectedEntities.OfType<IIfcClippable>();
            foreach (IIfcClippable ifcClippable in clippables)
            {
                ifcClippable.Clip(NodeEntity, Length/ 2);
            }
        }
    }
}