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

public class IfcValveEntity : IfcAbstractEntity
{
    private const int _numSegments = 32;
    private const double _angleStep = 2 * Math.PI / _numSegments;
    
    private readonly StartValveEntity _startValveEntity;
    private readonly IfcNodeEntity _ifcNodeEntity;
    private readonly IfcPipeEntity[] _ifcPipeEntities;

    private IfcPipeFitting? _pipeFitting;

    private double Length;
    private double Diameter;

    public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

    public IfcValveEntity(StartValveEntity startValveEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities)
    {
        _startValveEntity = startValveEntity;
        _ifcNodeEntity = nodeEntity;
        _ifcPipeEntities = pipeEntities;

        XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
        XbimVector3D forward = _ifcPipeEntities[0].ObjectMatrix3D.Forward;
        XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
        if (forward == WorldUp || forward == -1 * WorldUp) 
            WorldUp = new XbimVector3D(0, 1, 0);
        XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);

        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);

        Length = _startValveEntity.GetLength();
        Diameter = _startValveEntity.GetOutsideDiameter();
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        IfcCartesianPoint point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        IfcDirection forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
        IfcDirection rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);
        IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point, forwardDirection, rightDirection);
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);

        IfcBooleanResult result = CreateValve(model);
        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, result);
        IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
            fitting.Name = _startValveEntity.GetName();
            fitting.Representation = shape;
            fitting.Tag = "Valve";
            fitting.ObjectPlacement = localPlacement;
        });
        _ifcPipeEntities[0].Clip(_ifcNodeEntity, Length / 2);
        _ifcPipeEntities[1].Clip(_ifcNodeEntity, Length / 2);
        
        AddProperties(model, _pipeFitting);
        ConnectPorts(model);

        return _pipeFitting;
    }
    
    private IfcCartesianPoint[] CreateCircle(IModel model, double radius, double height)
    {
        IfcCartesianPoint[] points = new IfcCartesianPoint[_numSegments];
        for (int i = 0; i < _numSegments; i++)
        {
            XbimVector3D point = new XbimVector3D(
                radius * Math.Cos(_angleStep * i),
                radius * Math.Sin(_angleStep * i),
                height
            );
            points[i] = IfcAxis.CreatePoint(model, point);
        }

        return points;
    }
    
    protected IfcRelConnectsPorts ConnectPorts(IModel model)
    {
        var closestPorts = (
            from port in _ifcPipeEntities.SelectMany(pipe => pipe.Ports)
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

    private IfcBooleanResult CreateValve(IModel model)
    {
        IfcCartesianPoint[] firstCircle = CreateCircle(model, Diameter / 2, -Length / 2);
        IfcCartesianPoint[] secondCircle = CreateCircle(model, Diameter / 2, Length / 2);
        IfcCartesianPoint topPoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
        IfcFacetedBrep lowerBrep = CreateFacetedBrep(model, firstCircle, topPoint);
        IfcFacetedBrep upperBrep = CreateFacetedBrep(model, secondCircle, topPoint);

        return model.Instances.New<IfcBooleanResult>(booleanResult =>
        {
            booleanResult.Operator = IfcBooleanOperator.UNION;
            booleanResult.FirstOperand = lowerBrep;
            booleanResult.SecondOperand = upperBrep;
        });
    }
    
    private static IfcFacetedBrep CreateFacetedBrep(IModel model, IfcCartesianPoint[] points, IfcCartesianPoint topPoint)
    {
        IfcFace[] faces = new IfcFace[_numSegments + 1];
        int facesIndex = 0;
        for (int i = 0; i < _numSegments; i++)
        {
            IfcCartesianPoint p1 = points[i];
            IfcCartesianPoint p2 = points[(i + 1) % _numSegments];
            IfcCartesianPoint p3 = topPoint;
            faces[facesIndex++] = IfcGeometry.CreateTriangleFace(model, p1, p2, p3);
        }
        faces[facesIndex++] = IfcGeometry.CreatePolygonFace(model, points);

        return model.Instances.New<IfcFacetedBrep>(brep =>
        {
            brep.Outer = model.Instances.New<IfcClosedShell>(closedShell => closedShell.CfsFaces.AddRange(faces));
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
                foreach (var kvp in _startValveEntity.GetData())
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