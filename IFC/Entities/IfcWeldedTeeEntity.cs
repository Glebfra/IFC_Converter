using IFC_Converter.IFC.Tools;
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

    public XbimMatrix3D ObjectMatrix3D { get; }

    public IfcWeldedTeeEntity(StartWeldedTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes)
    {
        _teeEntity = teeEntity;
        _connPipes = connPipes;
        _nodeEntity = nodeEntity;

        ObjectMatrix3D = XbimMatrix3D.CreateWorld(_nodeEntity.Coordinates, new XbimVector3D(1, 0, 0),
            new XbimVector3D(0, 0, 1));
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        SortPipes(out IfcPipeEntity[] branchPipes, out IfcPipeEntity headPipe);
        IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_connPipes.Length];

        int i = 0;
        foreach (var branchPipe in branchPipes)
        {
            XbimVector3D weldedTeeBranchDirection = IfcAxis.GetDirectionToPipe(branchPipe, ObjectMatrix3D.Translation);
            IfcAxis2Placement3D teeBranchAxis =
                IfcAxis.CreateAxis2Placement3D(model, new XbimVector3D(), weldedTeeBranchDirection);
            teeExtrudedArea[i++] = CreateTeeItemShape(model, teeBranchAxis, branchPipe.Diameter / 2,
                _teeEntity.GetBranchHeight() / 2);

            branchPipe.Clip(model, _nodeEntity, _teeEntity.GetBranchHeight() / 2);
        }

        XbimVector3D teeBranchDirection = IfcAxis.GetDirectionToPipe(headPipe, ObjectMatrix3D.Translation);
        IfcAxis2Placement3D teeHeadAxis = IfcAxis.CreateAxis2Placement3D(model, new XbimVector3D(), teeBranchDirection);
        teeExtrudedArea[i++] =
            CreateTeeItemShape(model, teeHeadAxis, headPipe.Diameter / 2, _teeEntity.GetHeaderLength());
        headPipe.Clip(model, _nodeEntity, _teeEntity.GetHeaderLength());

        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, teeExtrudedArea);
        IfcProductDefinitionShape productDefinitionShape =
            IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        IfcPipeFitting pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.Name = _teeEntity.GetName();
            fitting.PredefinedType = IfcPipeFittingTypeEnum.JUNCTION;
            fitting.Representation = productDefinitionShape;
            fitting.ObjectPlacement = IfcAxis.CreateLocalPlacement(model, ObjectMatrix3D.Translation);
        });
        IfcProperty.AddProperties(model, "Pset_PipeFittingCommon", pipeFitting, _teeEntity.GetData());

        model.Instances.New<IfcRelNests>(nests =>
        {
            nests.Name = "Port";
            nests.Description = "Connects bend and node";
            nests.RelatingObject = pipeFitting;
            nests.RelatedObjects.Add(_nodeEntity.Port);
        });

        return pipeFitting;
    }

    private void SortPipes(out IfcPipeEntity[] branchPipes, out IfcPipeEntity headPipe)
    {
        branchPipes = new IfcPipeEntity[2];
        headPipe = null;

        for (int j = 0; j < _connPipes.Length; j++)
        {
            for (int k = j + 1; k < _connPipes.Length; k++)
            {
                XbimVector3D firstPipeDir = _connPipes[j].ObjectMatrix3D.Forward;
                XbimVector3D secondPipeDir = _connPipes[k].ObjectMatrix3D.Forward;

                double angleCos = XbimVector3D.DotProduct(firstPipeDir, secondPipeDir) /
                                  (firstPipeDir.Length * secondPipeDir.Length);

                if (System.Math.Abs(angleCos) < 0.95) continue;
                branchPipes[0] = _connPipes[j];
                branchPipes[1] = _connPipes[k];
                headPipe = _connPipes[^(j + k)];
            }
        }
    }

    private IfcExtrudedAreaSolid CreateTeeItemShape(IModel model, IfcAxis2Placement3D axis, double radius,
        double length)
    {
        IfcCircleProfileDef profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = radius;
        });

        IfcExtrudedAreaSolid extrudedAreaSolid = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.ExtrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            solid.Depth = length;
            solid.Position = axis;
        });

        return extrudedAreaSolid;
    }
}