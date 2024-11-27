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

    public override IfcObject CreateAndAdd(IModel model)
    {
        SortPipes(out StartPipeEntity[] branchPipes, out StartPipeEntity headPipe);
        IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_connPipes.Length];
        
        int i = 0;
        foreach (var branchPipe in branchPipes)
        {
            Vector3 weldedTeeBranchDirection = GetRightPipeDirection(branchPipe, Coordinates);
            IfcAxis2Placement3D teeBranchAxis = CreateAxis2Placement3D(model, new Vector3(), weldedTeeBranchDirection);
            teeExtrudedArea[i++] = CreateTeeItemShape(model, teeBranchAxis, branchPipe.GetOutsideDiameter() / 2, _teeEntity.GetBranchHeight() / 2);
        }
        
        Vector3 teeBranchDirection = GetRightPipeDirection(headPipe, Coordinates);
        IfcAxis2Placement3D teeHeadAxis = CreateAxis2Placement3D(model, new Vector3(), teeBranchDirection);
        teeExtrudedArea[i++] = CreateTeeItemShape(model, teeHeadAxis, headPipe.GetOutsideDiameter() / 2, _teeEntity.GetHeaderLength());

        IfcShapeRepresentation shapeRepresentation = CreateShapeRepresentation(model, teeExtrudedArea);
        IfcProductDefinitionShape productDefinitionShape = CreateProductDefinitionShape(model, shapeRepresentation);
        IfcPipeSegment pipe = model.Instances.New<IfcPipeSegment>(segment =>
        {
            segment.Name = _teeEntity.GetName();
            segment.Representation = productDefinitionShape;
            segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            segment.ObjectPlacement = CreateLocalPlacement(model, Coordinates);
        });

        AddProperties(model, pipe, _teeEntity);

        return pipe;
    }

    private void SortPipes(out StartPipeEntity[] branchPipes, out StartPipeEntity headPipe)
    {
        branchPipes = new StartPipeEntity[2];
        headPipe = null;

        for (int j = 0; j < _connPipes.Length; j++)
        {
            for (int k = j + 1; k < _connPipes.Length; k++)
            {
                Vector3 firstPipeDir = _connPipes[j].GetDirection();
                Vector3 secondPipeDir = _connPipes[k].GetDirection();

                double angleCos = Vector3.Dot(firstPipeDir, secondPipeDir) / (firstPipeDir.Length * secondPipeDir.Length);

                if (System.Math.Abs(angleCos) < 0.95) continue;
                branchPipes[0] = _connPipes[j];
                branchPipes[1] = _connPipes[k];
                headPipe = _connPipes[^(j + k)];
            }
        }
    }

    private IfcExtrudedAreaSolid CreateTeeItemShape(IModel model, IfcAxis2Placement3D axis, double radius, double length)
    {
        IfcCircleProfileDef profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = radius * 1.1;
        });

        IfcExtrudedAreaSolid extrudedAreaSolid = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.ExtrudedDirection = CreateDirection(model, new Vector3(0, 0, 1));
            solid.Depth = length;
            solid.Position = axis;
        });

        return extrudedAreaSolid;
    }
}