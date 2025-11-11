using System.Collections.Generic;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedComponentElements;

namespace IFC.Entities.Abstract.Anchors
{
    public abstract class IfcAbstractSpringSupportEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract int NumSegments { get; }
        public abstract double Height { get; }
        
        protected IfcAbstractSpringSupportEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcDiscreteAccessory discreteAccessory = CreateIfcEntity<IfcDiscreteAccessory>(model);
            return discreteAccessory;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcDiscreteAccessory, IInstantiableEntity
        {
            T discreteAccessory = base.CreateIfcEntity<T>(model);
            discreteAccessory.PredefinedType = IfcDiscreteAccessoryTypeEnum.ANCHORPLATE;

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model, Diameter / 2 * VectorExtensions.Forward);
            AddShapeRepresentation(model, discreteAccessory, representationItems);
            
            return discreteAccessory;
        }
        
        protected override IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[4];

            XbimVector3D rectangleCoordinates = displacement - Height * VectorExtensions.Forward;
            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);

            XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            double stickRadius = Diameter / 10;
            double stickHeight = Height / 3;
            representationItems[1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            double springRadius = stickRadius * 2;
            double springWireRadius = stickRadius / 2;
            const int springNumTurns = 5;
            IfcCartesianPoint[] spiralPoints = IfcGeometry.CreateSpiral(model, springRadius, stickHeight, NumSegments, springNumTurns, stickCoordinates);
            IfcPolyline spiralPolyline = IfcGeometry.CreatePolyline(model, spiralPoints);
            representationItems[2] = IfcGeometry.CreateSweptDiskSolid(model, spiralPolyline, springWireRadius);

            XbimVector3D coneCoordinates = stickCoordinates + stickHeight * VectorExtensions.Forward;
            XbimVector3D coneTopCoordinates = displacement - Diameter / 2 * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] circle = IfcGeometry.CreateCircle(model, coneRadius, coneCoordinates, NumSegments);
            IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
            representationItems[3] = IfcGeometry.CreateCone(model, circle, topPoint);

            return representationItems;
        }
    }
}