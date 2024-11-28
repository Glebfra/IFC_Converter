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
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public sealed class IfcPipeEntity : IfcAbstractEntity
{
    private readonly StartPipeEntity _pipeEntity;

    private readonly XbimVector3D _startCoordinates;
    private readonly XbimVector3D _direction;
    private readonly XbimMatrix3D _worldMatrix3D;
    private readonly double _diameter;

    public IfcPipeEntity(StartPipeEntity pipeEntity)
    {
        _pipeEntity = pipeEntity;
        _startCoordinates = _pipeEntity.GetCoordinates();
        _direction = _pipeEntity.GetDirection();
        _diameter = _pipeEntity.GetOutsideDiameter();
        _worldMatrix3D = XbimMatrix3D.CreateWorld(_startCoordinates, _direction.Normalized(), new XbimVector3D(_direction.Y, _direction.Z, _direction.X).Normalized());
    }

    public override IfcObject CreateAndAdd(IModel model)
    {
        IfcLocalPlacement localStartPlacement = CreateLocalPlacement(model, _startCoordinates);
        
        IfcProductDefinitionShape productDefShape = CreatePipeShape(model);
        IfcPipeSegment pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = _pipeEntity.GetName();
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = localStartPlacement;
            p.Representation = productDefShape;
        });
        AddProperties(model, pipeSegment, _pipeEntity);

        return pipeSegment;
    }

    private IfcProductDefinitionShape CreatePipeShape(IModel model)
    {
        IfcCircleProfileDef profileDef = CreateCircleProfileDef(model, _diameter / 2, XbimVector3D.Zero, new XbimVector3D(1, 0, 0));
        IfcExtrudedAreaSolid extrudedSolid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1));
            s.Depth = _direction.Length;
            s.Position = CreateAxis2Placement3D(model, XbimVector3D.Zero, _worldMatrix3D.Forward);
        });
        IfcShapeRepresentation shapeRep = CreateShapeRepresentation(model, extrudedSolid);
        IfcProductDefinitionShape productDefShape = CreateProductDefinitionShape(model, shapeRep);

        return productDefShape;
    }
}