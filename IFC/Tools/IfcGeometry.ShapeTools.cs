using System.Collections.Generic;
using System.Linq;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Tools
{
    public static partial class IfcGeometry
    {
        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IfcRepresentationItem representationItem)
        {
            return CreateShapeRepresentation(model, new[] { representationItem });
        }

        public static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IEnumerable<IfcRepresentationItem> representationItems)
        {
            return model.Instances.New<IfcShapeRepresentation>(representation =>
            {
                representation.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
                representation.RepresentationIdentifier = "Body";
                representation.RepresentationType = "SweptSolid";
                representation.Items.AddRange(representationItems);
            });
        }

        public static IfcProductDefinitionShape CreateProductDefinitionShape(IModel model, IfcShapeRepresentation shapeRepresentation)
        {
            return model.Instances.New<IfcProductDefinitionShape>(shape => shape.Representations.Add(shapeRepresentation));
        }
        
        public static IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcProfileDef profileDef, double zDim, XbimVector3D coordinates)
        {
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, coordinates);
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
            IfcDirection extrudeDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);

            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = zDim;
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudeDirection;
                solid.Position = axis2Placement3D;
            });
        }
        
        public static IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcProfileDef profileDef, double zDim, XbimVector3D coordinates, XbimVector3D axis, XbimVector3D refDirection)
        {
            IfcCartesianPoint point = IfcAxis.CreatePoint(model, coordinates);
            IfcDirection ifcAxis = IfcAxis.CreateDirection(model, axis);
            IfcDirection ifcRefDirection = IfcAxis.CreateDirection(model, refDirection);
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point, ifcAxis, ifcRefDirection);
            IfcDirection extrudeDirection = IfcAxis.CreateDirection(model, VectorExtensions.Forward);

            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = zDim;
                solid.SweptArea = profileDef;
                solid.ExtrudedDirection = extrudeDirection;
                solid.Position = axis2Placement3D;
            });
        }
        
        public static IfcSweptDiskSolid CreateSweptDiskSolid(IModel model, IfcCurve curve, IfcPositiveLengthMeasure radius)
        {
            return model.Instances.New<IfcSweptDiskSolid>(solid =>
            {
                solid.Directrix = curve;
                solid.Radius = radius;
            });
        }
    }
}