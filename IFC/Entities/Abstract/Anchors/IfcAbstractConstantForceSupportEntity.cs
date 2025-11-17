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
    public abstract class IfcAbstractConstantForceSupportEntity : IfcAbstractNonFixedSupportEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }
        public abstract ActionProperty<double> Height { get; }
        
        protected IfcAbstractConstantForceSupportEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }

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
            
            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchorModel(model, Diameter / 2 * VectorExtensions.Forward);
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
            
            double coneHeight = Height / 4;
            XbimVector3D botConeTopCoordinates = rectangleCoordinates + rectangleHeight * VectorExtensions.Forward;
            XbimVector3D botConeCoordinates = botConeTopCoordinates + coneHeight * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] botConeCircle = IfcGeometry.CreateCircle(model, coneRadius, botConeCoordinates, NumSegments);
            IfcCartesianPoint botConeTopPoint = IfcAxis.CreatePoint(model, botConeTopCoordinates);
            representationItems[1] = IfcGeometry.CreateCone(model, botConeCircle, botConeTopPoint);

            XbimVector3D stickCoordinates = botConeCoordinates;
            double stickRadius = Diameter / 10;
            double stickHeight = Height - rectangleHeight - 2 * coneHeight - Diameter / 2;
            representationItems[2] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            XbimVector3D topConeCoordinates = stickCoordinates + stickHeight * VectorExtensions.Forward;
            XbimVector3D topConeTopCoordinates = topConeCoordinates + coneHeight * VectorExtensions.Forward;
            double topConeRadius = Diameter / 4;
            IfcCartesianPoint[] topConeCircle = IfcGeometry.CreateCircle(model, topConeRadius, topConeCoordinates, NumSegments);
            IfcCartesianPoint topConeTopPoint = IfcAxis.CreatePoint(model, topConeTopCoordinates);
            representationItems[3] = IfcGeometry.CreateCone(model, topConeCircle, topConeTopPoint);

            return representationItems;
        }
    }
}