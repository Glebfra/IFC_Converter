using IFC_Converter.IFC.Entities.Abstract;
using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC_Converter.IFC.Entities;

public class IfcReducerEccentricEntity : IfcAbstractReducerEntity
{
    private const int _numSegments = 32;
    private const double _angleStep = 2 * Math.PI / _numSegments;
    protected override IfcPipeFitting? _pipeFitting { get; set; }

    public IfcReducerEccentricEntity(StartReducerEccentricEntity startReducerEccentric, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities) 
        : base(startReducerEccentric, nodeEntity, pipeEntities)
    {
    }
    
    public override IfcProduct CreateAndAdd(IModel model)
    {
        double firstRadius = _pipeEntities[0].Diameter / 2;
        double secondRadius = _pipeEntities[1].Diameter / 2;
        double minRadius = Math.Min(firstRadius, secondRadius);
        
        IfcCartesianPoint point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        IfcDirection forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
        IfcDirection rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);
        IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point, forwardDirection, rightDirection);
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);
        
        IfcCartesianPoint[] lowerCircle = CreateCircle(model, firstRadius, 0, firstRadius - minRadius);
        IfcCartesianPoint[] upperCircle = CreateCircle(model, secondRadius, Length, secondRadius - minRadius);
        IfcFacetedBrep facetedBrep = CreateFacetedBrep(model, lowerCircle, upperCircle);
        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, facetedBrep);
        IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        
        _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.ObjectPlacement = localPlacement;
            fitting.Representation = shape;
            fitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;
            fitting.Tag = "Reducer";
            fitting.Name = _startReducer.GetName();
        });
        _pipeEntities[1].Clip(_nodeEntity, Length);
        
        AddProperties(model);
        ConnectPorts(model);

        return _pipeFitting;
    }

    private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height, double displacement)
    {
        IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
        for (int i = 0; i < _numSegments; i++)
        {
            XbimVector3D point = new XbimVector3D(radius * Math.Cos(_angleStep * i), radius * Math.Sin(_angleStep * i) - displacement, height);
            points[i] = IfcAxis.CreatePoint(model, point);
        }

        return points;
    }

    private IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] lowerPoints, IfcCartesianPoint[] upperPoints)
    {
        IfcFace[] faces = new IfcFace[_numSegments + 2];
        int facesIndex = 0;
        for (int i = 0; i < _numSegments; i++)
        {
            IfcCartesianPoint p1 = lowerPoints[i];
            IfcCartesianPoint p2 = lowerPoints[(i + 1) % _numSegments];
            IfcCartesianPoint p3 = upperPoints[(i + 1) % _numSegments];
            IfcCartesianPoint p4 = upperPoints[i];
            faces[facesIndex++] = IfcGeometry.CreateRectangleFace(model, p1, p2, p3, p4);
        }
        faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, lowerPoints);
        faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, upperPoints);

        return model.Instances.New<IfcFacetedBrep>(brep =>
        {
            brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
        });
    }
}