using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.TopologyResource;

namespace IFC.Entities;

public class IfcReducerEccentricEntity : IfcAbstractEntity
{
    private const int _numSegments = 32;
    private const double _angleStep = 2 * Math.PI / _numSegments;

    private readonly double _angle;
    
    private IfcPipeFitting? _pipeFitting { get; set; }
    
    private readonly StartReducerEccentricEntity _startReducer;
    private readonly IfcPipeEntity[] _pipeEntities;
    private readonly IfcNodeEntity _nodeEntity;

    public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
    public double Length { get; }

    public IfcReducerEccentricEntity(StartReducerEccentricEntity startReducerEccentric, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities)
    {
        _startReducer = startReducerEccentric;
        _nodeEntity = nodeEntity;
        _pipeEntities = pipeEntities;
        _nodeEntity.ConnEntities.Add(this);
        
        XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
        XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(pipeEntities[1], coordinates);
        XbimMatrix3D[] ObjectMatrices = _pipeEntities.Select(entity => entity.ObjectMatrix3D).ToArray();
        
        XbimVector3D forward = directionToPipe.Normalized();
        XbimVector3D up = _pipeEntities[0].Diameter < _pipeEntities[1].Diameter
            ? ObjectMatrices[1].Translation - ObjectMatrices[0].Translation
            : ObjectMatrices[0].Translation - ObjectMatrices[1].Translation;
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        Length = _startReducer.GetLengthOfConicalPart();
        
        _angle = startReducerEccentric.GetAngleBetweenEccentricityVectorAndZmAxis();
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

        AddProperties(model, _pipeFitting);
        ConnectPorts(model);

        return _pipeFitting;
    }

    private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height, double displacement)
    {
        IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
        for (int i = 0; i < _numSegments; i++)
        {
            XbimVector3D point = new XbimVector3D(
                radius * Math.Cos(_angleStep * i),
                radius * Math.Sin(_angleStep * i) - displacement,
                height
            );
            points[i] = IfcAxis.CreatePoint(model, point);
        }

        return points;
    }

    private static IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] lowerPoints, IfcCartesianPoint[] upperPoints)
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
    
    protected IfcRelConnectsPorts ConnectPorts(IModel model)
    {
        var closestPorts = (
            from port in _pipeEntities.SelectMany(pipe => pipe.Ports)
            let distance = (port.ObjectPlacement.ToMatrix3D().Translation - ObjectMatrix3D.Translation).Length
            orderby distance
            select port
        ).Take(2).ToArray();

        return model.Instances.New<IfcRelConnectsPorts>(ports =>
        {
            ports.Name = $"{closestPorts[0].GlobalId}|{closestPorts[1].GlobalId}";
            ports.Description = "Flow";
            ports.RelatingPort = closestPorts[0];
            ports.RelatedPort = closestPorts[1];
            ports.RealizingElement = _pipeFitting;
        });
    }

    protected override void AddProperties(IModel model, IfcProduct product)
    {
        base.AddProperties(model, product);
        
        #region Pset_PipeFittingTypeStart
        
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(product);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeStart";
                foreach (var kvp in _startReducer.GetData())
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = kvp.Key;
                        value.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
        });

        #endregion
    }
}