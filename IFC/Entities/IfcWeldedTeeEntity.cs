using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
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
    private readonly IfcPipeEntity[] _connPipes;
    private readonly IfcNodeEntity _nodeEntity;

    public XbimVector3D Coordinates { get; }

    public IfcWeldedTeeEntity(StartWeldedTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes)
    {
        _teeEntity = teeEntity;
        _connPipes = connPipes;
        _nodeEntity = nodeEntity;

        Coordinates = _nodeEntity.Coordinates;
    }

    public override IfcObject CreateAndAdd(IModel model)
    {
        SortPipes(out IfcPipeEntity[] branchPipes, out IfcPipeEntity headPipe);
        IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_connPipes.Length];
        
        int i = 0;
        foreach (var branchPipe in branchPipes)
        {
            XbimVector3D weldedTeeBranchDirection = GetDirectionToPipe(branchPipe, Coordinates);
            IfcAxis2Placement3D teeBranchAxis = CreateAxis2Placement3D(model, new XbimVector3D(), weldedTeeBranchDirection);
            teeExtrudedArea[i++] = CreateTeeItemShape(model, teeBranchAxis, branchPipe.Diameter / 2, _teeEntity.GetBranchHeight() / 2);
            
            branchPipe.Clip(model, _nodeEntity, _teeEntity.GetBranchHeight() / 2);
        }
        
        XbimVector3D teeBranchDirection = GetDirectionToPipe(headPipe, Coordinates);
        IfcAxis2Placement3D teeHeadAxis = CreateAxis2Placement3D(model, new XbimVector3D(), teeBranchDirection);
        teeExtrudedArea[i++] = CreateTeeItemShape(model, teeHeadAxis, headPipe.Diameter / 2, _teeEntity.GetHeaderLength());
        headPipe.Clip(model, _nodeEntity, _teeEntity.GetHeaderLength());

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

    private void ClipPipes(IModel model)
    {
        
    }

    private void SortPipes(out IfcPipeEntity[] branchPipes, out IfcPipeEntity headPipe)
    {
        branchPipes = new IfcPipeEntity[2];
        headPipe = null;

        for (int j = 0; j < _connPipes.Length; j++)
        {
            for (int k = j + 1; k < _connPipes.Length; k++)
            {
                XbimVector3D firstPipeDir = _connPipes[j].Direction;
                XbimVector3D secondPipeDir = _connPipes[k].Direction;

                double angleCos = XbimVector3D.DotProduct(firstPipeDir, secondPipeDir) / (firstPipeDir.Length * secondPipeDir.Length);

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
            c.Radius = radius;
        });

        IfcExtrudedAreaSolid extrudedAreaSolid = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.ExtrudedDirection = CreateDirection(model, new XbimVector3D(0, 0, 1));
            solid.Depth = length;
            solid.Position = axis;
        });

        return extrudedAreaSolid;
    }
}