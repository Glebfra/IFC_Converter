using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Common.Geometry;

namespace IFC_Converter.IFC.Entities;

public class IfcPipeEntity : IfcAbstractEntity
{
    public StartPipeEntity PipeEntity { get; }

    public XbimVector3D StartCoordinates { get; }
    public XbimVector3D Direction { get; }
    public XbimMatrix3D ObjectMatrix3D { get; }
    public double Diameter { get; }

    private IfcNodeEntity[] _nodeEntities;
    private IfcExtrudedAreaSolid _extrudedArea;
    private IfcPipeSegment _pipeSegment;

    public IfcPipeEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] ifcNodeEntities)
    {
        PipeEntity = pipeEntity;
        _nodeEntities = ifcNodeEntities;
        StartCoordinates = PipeEntity.GetCoordinates();
        Direction = PipeEntity.GetDirection();
        Diameter = PipeEntity.GetOutsideDiameter();
        
        XbimVector3D forward = Direction.Normalized();
        XbimVector3D up = XbimVector3D.CrossProduct(forward, new XbimVector3D(0, 0, 1));
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(StartCoordinates, forward, up);
    }

    public override IfcObject CreateAndAdd(IModel model)
    {
        IfcLocalPlacement localStartPlacement = IfcAxis.CreateLocalPlacement(model, StartCoordinates);
        
        IfcProductDefinitionShape productDefShape = CreatePipeShape(model);
        _pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = PipeEntity.GetName();
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = localStartPlacement;
            p.Representation = productDefShape;
        });
        IfcProperty.AddProperties(model, _pipeSegment, PipeEntity);

        model.Instances.New<IfcRelNests>(nests =>
        {
            nests.Name = "Pipe ports";
            nests.Description = "Connects two ports of the pipe";
            nests.RelatingObject = _pipeSegment;
            nests.RelatedObjects.AddRange(_nodeEntities.Select(nodeEntity => nodeEntity.Port));
        });

        return _pipeSegment;
    }

    public void Clip(IModel model, IfcNodeEntity nodeEntity, double clipLength)
    {
        if ((nodeEntity.Coordinates - StartCoordinates).Length < (nodeEntity.Coordinates - StartCoordinates - Direction).Length)
            _extrudedArea.Position = IfcAxis.CreateAxis2Placement3D(model, ObjectMatrix3D.Forward * clipLength, ObjectMatrix3D.Forward);
        _extrudedArea.Depth -= clipLength;
    }

    private IfcProductDefinitionShape CreatePipeShape(IModel model)
    {
        IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
        _extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            s.Depth = Direction.Length;
            s.Position = IfcAxis.CreateAxis2Placement3D(model, XbimVector3D.Zero, ObjectMatrix3D.Forward, ObjectMatrix3D.Right);
        });
        IfcShapeRepresentation shapeRep = IfcGeometry.CreateShapeRepresentation(model, _extrudedArea);
        IfcProductDefinitionShape productDefShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRep);

        return productDefShape;
    }
}