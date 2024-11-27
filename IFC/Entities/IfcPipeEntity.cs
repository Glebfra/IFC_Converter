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

    public XbimVector3D StartCoordinates;
    public XbimVector3D EndCoordinates;
    public XbimVector3D Direction;
    public double Diameter;

    public IfcPipeEntity(StartPipeEntity pipeEntity)
    {
        _pipeEntity = pipeEntity;
        StartCoordinates = _pipeEntity.GetCoordinates();
        Direction = _pipeEntity.GetDirection();
        EndCoordinates = StartCoordinates + Direction;
        Diameter = _pipeEntity.GetOutsideDiameter();
    }

    public override IfcObject CreateAndAdd(IModel model)
    {
        IfcLocalPlacement localStartPlacement = CreateLocalPlacement(model, StartCoordinates, Direction);
        IfcLocalPlacement localEndPlacement = CreateLocalPlacement(model, EndCoordinates, Direction);

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
        IfcCircleProfileDef profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = Diameter / 2;
        });

        IfcExtrudedAreaSolid extrudedSolid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1));
            s.Depth = Direction.Length;
        });

        IfcShapeRepresentation shapeRep = CreateShapeRepresentation(model, extrudedSolid);
        IfcProductDefinitionShape productDefShape = CreateProductDefinitionShape(model, shapeRep);

        return productDefShape;
    }
}