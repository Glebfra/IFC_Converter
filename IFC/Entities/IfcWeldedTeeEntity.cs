using IFC_Converter.Math;
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

namespace IFC_Converter.IFC.Entities;

public class IfcWeldedTeeEntity : IfcAbstractEntity
{
    private readonly StartWeldedTeeEntity _teeEntity;
    private readonly StartPipeEntity[] _connPipes;

    public Vector3 Coordinates;

    public IfcWeldedTeeEntity(StartWeldedTeeEntity teeEntity, StartNodeEntity nodeEntity, StartPipeEntity[] connPipes)
    {
        _teeEntity = teeEntity;
        _connPipes = connPipes;

        Coordinates = nodeEntity.GetCoordinates();
    }

    public override void CreateAndAdd(IModel model)
    {
        IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_connPipes.Length];

        int i = 0;
        foreach (var pipeEntity in _connPipes)
        {
            Vector3 pipeStartCoordinates = pipeEntity.GetCoordinates();
            Vector3 pipeDirection = pipeEntity.GetDirection();
            Vector3 pipeEndCoordinates = pipeStartCoordinates + pipeDirection;
            Vector3 weldedTeeBranchDirection =
                (pipeStartCoordinates - Coordinates).Length() < (pipeEndCoordinates - Coordinates).Length()
                    ? pipeDirection
                    : pipeDirection * -1;

            IfcLocalPlacement localStartPlacement =
                CreateLocalPlacementAndDirection(model, Coordinates, weldedTeeBranchDirection);

            IfcAxis2Placement3D teeBranchAxis = CreateAxis2Placement3D(model, Coordinates, weldedTeeBranchDirection);
            teeExtrudedArea[i++] = CreateTeeBranchShape(model, pipeEntity, teeBranchAxis);
        }

        IfcShapeRepresentation shapeRepresentation = model.Instances.New<IfcShapeRepresentation>(representation =>
        {
            representation.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            representation.RepresentationIdentifier = "Body";
            representation.RepresentationType = "SweptSolid";
            representation.Items.AddRange(teeExtrudedArea);
        });

        IfcProductDefinitionShape productDefinitionShape = model.Instances.New<IfcProductDefinitionShape>(shape =>
        {
            shape.Representations.Add(shapeRepresentation);
        });

        IfcPipeSegment pipe = model.Instances.New<IfcPipeSegment>(segment =>
        {
            segment.Name = _teeEntity.GetName();
            segment.Representation = productDefinitionShape;
            segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
        });

        AddProperties(model, pipe, _teeEntity);
    }

    private IfcExtrudedAreaSolid CreateTeeBranchShape(IModel model, StartPipeEntity startPipeEntity, IfcAxis2Placement3D axis)
    {
        IfcCircleProfileDef profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = startPipeEntity.GetOutsideDiameter() / 2 * 1.1;
        });

        IfcExtrudedAreaSolid extrudedAreaSolid = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.ExtrudedDirection = CreateDirection(model, new Vector3(0, 0, 1));
            solid.Depth = _teeEntity.GetBranchHeight() / 2;
            solid.Position = axis;
        });

        return extrudedAreaSolid;
    }
}