using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractStraightSegment : IfcAbstractSegmentEntity
    {
        protected IfcAbstractStraightSegment(StartAbstractSegmentEntity segmentEntity, IfcNodeEntity[] nodeEntities) 
            : base(segmentEntity, nodeEntities)
        {
            
        }
        
        protected IfcPipeSegment CreatePipeSegment(IModel model, string name, IfcPipeSegmentTypeEnum segmentTypeEnum)
        {
            IfcCartesianPoint startPoint = CreateStartPoint(model);
            IfcDirection forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
            IfcDirection rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);

            IfcAxis2Placement3D startAxis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, startPoint, forwardDirection, rightDirection);
            IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, startAxis2Placement3D);

            IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            IfcProductDefinitionShape productDefShape = CreatePipeShape(model, extrudedDirection);
            return CreatePipe(model, productDefShape, localPlacement, name, segmentTypeEnum);
        }
        
        protected IfcPipeSegment CreatePipe(IModel model, IfcProductDefinitionShape productDefShape, IfcLocalPlacement localPlacement, string name, IfcPipeSegmentTypeEnum segmentTypeEnum)
        {
            return model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Tag = Tag;
                segment.Name = name;
                segment.PredefinedType = segmentTypeEnum;
                segment.ObjectPlacement = localPlacement;
                segment.Representation = productDefShape;
            });
        }
        
        protected IfcProductDefinitionShape CreatePipeShape(IModel model, IfcDirection extrudedDirection)
        {
            IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
            IfcExtrudedAreaSolid extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudedDirection;
                solid.Depth = RealLength.Value;

                RealLength.OnValueChange += () => solid.Depth = RealLength.Value;
            });
            IfcShapeRepresentation shapeRep = IfcGeometry.CreateShapeRepresentation(model, extrudedArea, IfcRepresentationType.SweptSolid, IfcRepresentationIdentifier.Body);
        
            return IfcGeometry.CreateProductDefinitionShape(model, shapeRep);
        }

        protected IfcCartesianPoint CreateStartPoint(IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point =>
            {
                point.SetXYZ(Coordinates.Value.X, Coordinates.Value.Y, Coordinates.Value.Z);
                Coordinates.OnValueChange += () => point.SetXYZ(Coordinates.Value.X, Coordinates.Value.Y, Coordinates.Value.Z);
            });
        }
    }
}