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
    public abstract class IfcAbstractGuideDoubleDirectionSupportEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }
        public abstract ActionProperty<double> Height { get; }
        
        protected IfcAbstractGuideDoubleDirectionSupportEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }

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

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, XbimVector3D.Zero);
            AddShapeRepresentation(model, discreteAccessory, representationItems);
            
            return discreteAccessory;
        }

        protected override IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            XbimVector3D[] zDirections = new[] { VectorExtensions.Up, VectorExtensions.Up.Negated(), VectorExtensions.Right, VectorExtensions.Right.Negated() };
            XbimVector3D[] xDirections = new[] { VectorExtensions.Forward.Negated(), VectorExtensions.Forward, VectorExtensions.Forward, VectorExtensions.Forward.Negated() };
            XbimVector3D[] yDirections = new XbimVector3D[4];

            for (int i = 0; i < 4; i++)
            {
                yDirections[i] = XbimVector3D.CrossProduct(zDirections[i], xDirections[i]);
            }
            
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[zDirections.Length * 3];

            for (int i = 0; i < zDirections.Length; i++)
            {
                XbimVector3D rectangleCoordinates = displacement - Height * zDirections[i];
                double rectangleXDim = Diameter;
                double rectangleYDim = Diameter;
                double rectangleHeight = Height / 20;
                representationItems[i*3] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates, zDirections[i], xDirections[i]);
            
                XbimVector3D stickCoordinates = rectangleCoordinates + rectangleHeight * zDirections[i];
                double stickRadius = Diameter / 10;
                double stickHeight = Height / 3;
                representationItems[i*3 + 1] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates, zDirections[i], xDirections[i]);
            
                XbimVector3D coneCoordinates = stickCoordinates + stickHeight * zDirections[i];
                XbimVector3D coneTopCoordinates = displacement - Diameter / 2 * zDirections[i];
                double coneRadius = Diameter / 4;
                IfcCartesianPoint[] circle = IfcGeometry.CreateCircle(model, coneRadius, coneCoordinates, NumSegments, xDirections[i], yDirections[i]);
                IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, coneTopCoordinates);
                representationItems[i*3 + 2] = IfcGeometry.CreateCone(model, circle, topPoint);
            }
            
            return representationItems;
        }
    }
}