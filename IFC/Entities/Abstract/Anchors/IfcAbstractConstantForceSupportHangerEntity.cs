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
    public abstract class IfcAbstractConstantForceSupportHangerEntity : IfcAbstractConstantForceSupportEntity
    {
        protected IfcAbstractConstantForceSupportHangerEntity(XbimMatrix3D objectMatrix) : base(objectMatrix) { }
        
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

            IEnumerable<IfcRepresentationItem> representationItems = CreateAnchor(model, -Diameter / 2 * VectorExtensions.Forward);
            AddShapeRepresentation(model, discreteAccessory, representationItems);
            
            return discreteAccessory;
        }
        
        protected override IEnumerable<IfcRepresentationItem> CreateAnchorModel(IModel model, XbimVector3D displacement)
        {
            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[4];

            double rectangleXDim = Diameter;
            double rectangleYDim = Diameter;
            double rectangleHeight = Height / 20;
            XbimVector3D rectangleCoordinates = displacement + (Height - rectangleHeight) * VectorExtensions.Forward;
            representationItems[0] = IfcGeometry.CreateRectangle(model, rectangleXDim, rectangleYDim, rectangleHeight, rectangleCoordinates);
            
            double coneHeight = Height / 4;
            XbimVector3D botConeTopCoordinates = rectangleCoordinates;
            XbimVector3D botConeCoordinates = botConeTopCoordinates - coneHeight * VectorExtensions.Forward;
            double coneRadius = Diameter / 4;
            IfcCartesianPoint[] botConeCircle = IfcGeometry.CreateCircle(model, coneRadius, botConeCoordinates, NumSegments);
            IfcCartesianPoint botConeTopPoint = IfcAxis.CreatePoint(model, botConeTopCoordinates);
            representationItems[1] = IfcGeometry.CreateCone(model, botConeCircle, botConeTopPoint);

            double stickRadius = Diameter / 10;
            double stickHeight = Height - rectangleHeight - 2 * coneHeight - Diameter / 2;
            XbimVector3D stickCoordinates = botConeCoordinates - stickHeight * VectorExtensions.Forward;
            representationItems[2] = IfcGeometry.CreateCylinder(model, stickRadius, stickHeight, stickCoordinates);

            XbimVector3D topConeCoordinates = stickCoordinates;
            XbimVector3D topConeTopCoordinates = topConeCoordinates - coneHeight * VectorExtensions.Forward;
            double topConeRadius = Diameter / 4;
            IfcCartesianPoint[] topConeCircle = IfcGeometry.CreateCircle(model, topConeRadius, topConeCoordinates, NumSegments);
            IfcCartesianPoint topConeTopPoint = IfcAxis.CreatePoint(model, topConeTopCoordinates);
            representationItems[3] = IfcGeometry.CreateCone(model, topConeCircle, topConeTopPoint);

            return representationItems;
        }
    }
}