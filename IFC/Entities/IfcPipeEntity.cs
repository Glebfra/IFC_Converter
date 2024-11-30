using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
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
    public XbimMatrix3D WorldMatrix3D { get; }
    public double Diameter { get; }

    private IfcExtrudedAreaSolid _extrudedArea;
    private IfcPipeSegment _pipeSegment;

    public IfcPipeEntity(StartPipeEntity pipeEntity)
    {
        PipeEntity = pipeEntity;
        StartCoordinates = PipeEntity.GetCoordinates();
        Direction = PipeEntity.GetDirection();
        Diameter = PipeEntity.GetOutsideDiameter();
        WorldMatrix3D = XbimMatrix3D.CreateWorld(StartCoordinates, Direction.Normalized(), new XbimVector3D(Direction.Y, Direction.Z, Direction.X).Normalized());
    }

    public override IfcObject CreateAndAdd(IModel model)
    {
        IfcLocalPlacement localStartPlacement = CreateLocalPlacement(model, StartCoordinates);
        
        IfcProductDefinitionShape productDefShape = CreatePipeShape(model);
        _pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = PipeEntity.GetName();
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = localStartPlacement;
            p.Representation = productDefShape;
        });
        AddProperties(model, _pipeSegment, PipeEntity);

        return _pipeSegment;
    }

    public void Clip(IModel model, IfcNodeEntity nodeEntity, double clipLength)
    {
        if ((nodeEntity.Coordinates - StartCoordinates).Length < (nodeEntity.Coordinates - StartCoordinates - Direction).Length)
        {
            _extrudedArea.Position = CreateAxis2Placement3D(model, Direction.Normalized() * clipLength, WorldMatrix3D.Forward);
            _extrudedArea.Depth -= clipLength;
        }
        else
        {
            _extrudedArea.Depth -= clipLength;
        }
    }

    private IfcProductDefinitionShape CreatePipeShape(IModel model)
    {
        IfcCircleProfileDef profileDef = CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero, new XbimVector3D(1, 0, 0));
        _extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1));
            s.Depth = Direction.Length;
            s.Position = CreateAxis2Placement3D(model, XbimVector3D.Zero, WorldMatrix3D.Forward);
        });
        IfcShapeRepresentation shapeRep = CreateShapeRepresentation(model, _extrudedArea);
        IfcProductDefinitionShape productDefShape = CreateProductDefinitionShape(model, shapeRep);

        return productDefShape;
    }
}