using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC_Converter.IFC.Entities;

public class IfcWeldedTeeEntity : IfcAbstractEntity
{
    private readonly StartWeldedTeeEntity _teeEntity;
    private readonly IfcPipeEntity[] _connPipes;
    private readonly IfcNodeEntity _nodeEntity;

    private IfcPipeFitting _pipeFitting;

    public XbimMatrix3D ObjectMatrix3D { get; }

    public IfcWeldedTeeEntity(StartWeldedTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes)
    {
        _teeEntity = teeEntity;
        _connPipes = connPipes;
        _nodeEntity = nodeEntity;

        ObjectMatrix3D = XbimMatrix3D.CreateWorld(_nodeEntity.ObjectMatrix3D.Translation, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        SortPipes(out IfcPipeEntity[] branchPipes, out IfcPipeEntity headPipe);
        IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_connPipes.Length];

        int i = 0;
        foreach (var branchPipe in branchPipes)
        {
            teeExtrudedArea[i++] = CreateTeeBranchShape(model, branchPipe, _teeEntity.GetBranchHeight() / 2);
        }
        teeExtrudedArea[i++] = CreateTeeBranchShape(model, headPipe, _teeEntity.GetHeaderLength());

        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, teeExtrudedArea);
        IfcProductDefinitionShape productDefinitionShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.Name = _teeEntity.GetName();
            fitting.PredefinedType = IfcPipeFittingTypeEnum.JUNCTION;
            fitting.Representation = productDefinitionShape;
            fitting.ObjectPlacement = IfcAxis.CreateLocalPlacement(model, ObjectMatrix3D.Translation);
        });
        AddProperties(model);

        model.Instances.New<IfcRelNests>(nests =>
        {
            nests.Name = "Port";
            nests.Description = "Connects bend and node";
            nests.RelatingObject = _pipeFitting;
            nests.RelatedObjects.Add(_nodeEntity.Port);
        });

        return _pipeFitting;
    }

    private IfcExtrudedAreaSolid CreateTeeBranchShape(IModel model, IfcPipeEntity pipeEntity, double length)
    {
        XbimVector3D direction = IfcAxis.GetDirectionToPipe(pipeEntity, ObjectMatrix3D.Translation);
        IfcAxis2Placement3D axis = IfcAxis.CreateAxis2Placement3D(model, new XbimVector3D(), direction);
        IfcExtrudedAreaSolid extrudedAreaSolid = CreateTeeItemShape(model, axis, pipeEntity.Diameter / 2, length);
        pipeEntity.Clip(_nodeEntity, length);
        return extrudedAreaSolid;
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
            solid.ExtrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            solid.Depth = length;
            solid.Position = axis;
        });

        return extrudedAreaSolid;
    }
    
    private void AddProperties(IModel model)
    {
        #region DEBUG

        #if DEBUG
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Debug Properties";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Coordinates";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Translation.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Forward direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Forward.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Right direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Right.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Up direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Up.ToString());
                }));
            });
        });
        #endif

        #endregion
    }
}