using System;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractConeElementEntity : IfcAbstractSegmentEntity
    {
        public abstract double SecondDiameter { get; set; }
        
        protected abstract int _NumSegments { get; set; }
        private double _angleStep => 2 * Math.PI / _NumSegments;
        
        private StartConeElementEntity _coneElement;
        private IfcPipeSegment? _pipeSegment;
        
        protected IfcAbstractConeElementEntity(StartConeElementEntity coneElement, IfcNodeEntity[] nodeEntities) 
            : base(coneElement, nodeEntities)
        {
            _coneElement = coneElement;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcCartesianPoint[] firstCircle = CreateCircle(model, Diameter / 2, 0);
            IfcCartesianPoint[] secondCircle = CreateCircle(model, SecondDiameter / 2, Length.Value);
            IfcFacetedBrep facetedBrep = CreateFacetedBrep(model, firstCircle, secondCircle);
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBrep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);

            _pipeSegment = model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Tag = Tag;
                segment.Name = _coneElement.Name;
                segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                segment.ObjectPlacement = objectPlacement.LocalPlacement;
                segment.Representation = shape;
            });
            AddProperties(model, _pipeSegment);

            return _pipeSegment;
        }
        
        private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height)
        {
            IfcCartesianPoint[] points = new IfcCartesianPoint[_NumSegments];
            for (int i = 0; i < _NumSegments; i++)
            {
                XbimVector3D point = new XbimVector3D(radius * Math.Cos(_angleStep * i), radius * Math.Sin(_angleStep * i), height);
                points[i] = IfcAxis.CreatePoint(model, point);
            }

            return points;
        }
    
        private IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] lowerPoints, IfcCartesianPoint[] upperPoints)
        {
            IfcFace[] faces = new IfcFace[_NumSegments + 2];
            int facesIndex = 0;
            for (int i = 0; i < _NumSegments; i++)
            {
                IfcCartesianPoint p1 = lowerPoints[i];
                IfcCartesianPoint p2 = lowerPoints[(i + 1) % _NumSegments];
                IfcCartesianPoint p3 = upperPoints[(i + 1) % _NumSegments];
                IfcCartesianPoint p4 = upperPoints[i];
                faces[facesIndex++] = IfcVertexGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
            }
            faces[facesIndex++] = IfcVertexGeometry.CreatePolygonFace(model, lowerPoints);
            faces[facesIndex] = IfcVertexGeometry.CreatePolygonFace(model, upperPoints);

            return model.Instances.New<IfcFacetedBrep>(brep =>
            {
                brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
            });
        }
    }
}