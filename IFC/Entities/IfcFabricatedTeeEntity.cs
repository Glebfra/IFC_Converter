using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC_Converter.IFC.Entities;

public class IfcFabricatedTeeEntity : IfcAbstractTeeEntity
{
    public IfcFabricatedTeeEntity(StartFabricatedTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes) : base(connPipes)
    {
        _teeEntity = teeEntity;
        _nodeEntity = nodeEntity;
        
        Length = teeEntity.GetHeaderLength();
        Height = teeEntity.GetBranchHeight() + _branchPipes[0].Diameter / 2;

        ObjectMatrix3D = XbimMatrix3D.CreateWorld(_nodeEntity.ObjectMatrix3D.Translation, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        IfcCartesianPoint point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);
        IfcExtrudedAreaSolid[] teeExtrudedArea = new IfcExtrudedAreaSolid[_connPipes.Length];

        int i = 0;
        foreach (var branchPipe in _branchPipes)
        {
            teeExtrudedArea[i++] = CreateTeeBranchShape(model, branchPipe, Length / 2);
        }
        teeExtrudedArea[i++] = CreateTeeBranchShape(model, _headPipe, Height);

        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, teeExtrudedArea);
        IfcProductDefinitionShape productDefinitionShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.Name = Name;
            fitting.Tag = "WeldedTee";
            fitting.PredefinedType = IfcPipeFittingTypeEnum.JUNCTION;
            fitting.Representation = productDefinitionShape;
            fitting.ObjectPlacement = localPlacement;
        });
        AddProperties(model);
        ConnectPorts(model);

        return _pipeFitting;
    }
}