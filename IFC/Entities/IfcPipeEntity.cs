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
using Xbim.Ifc4.ProductExtension;

namespace IFC_Converter.IFC.Entities;

public class IfcPipeEntity : IfcAbstractEntity
{
    public StartPipeEntity PipeEntity { get; }
    public XbimMatrix3D ObjectMatrix3D { get; }
    public double Diameter { get; }
    public double Depth { get; }

    private IfcNodeEntity[] _nodeEntities;
    private IfcExtrudedAreaSolid _extrudedArea;
    private IfcPipeSegment _pipeSegment;

    public IfcPipeEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] ifcNodeEntities)
    {
        PipeEntity = pipeEntity;
        _nodeEntities = ifcNodeEntities;
        
        XbimVector3D direction = PipeEntity.GetDirection();
        Depth = direction.Length;
        Diameter = PipeEntity.GetOutsideDiameter();
        
        XbimVector3D forward = direction.Normalized();
        XbimVector3D up = XbimVector3D.CrossProduct(forward, new XbimVector3D(0, 0, 1));
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(PipeEntity.GetCoordinates(), forward, up);
    }

    public override IfcObject CreateAndAdd(IModel model)
    {
        IfcProductDefinitionShape productDefShape = CreatePipeShape(model);
        _pipeSegment = CreatePipe(model, productDefShape);
        IfcProperty.AddProperties(model, _pipeSegment, PipeEntity.GetData());

        model.Instances.New<IfcRelNests>(nests =>
        {
            nests.Name = "Pipe ports";
            nests.Description = "Connects two ports of the pipe";
            nests.RelatingObject = _pipeSegment;
            nests.RelatedObjects.AddRange(_nodeEntities.Select(nodeEntity => nodeEntity.Port));
        });

        IfcBuilding ifcBuilding = model.Instances.FirstOrDefault<IfcBuilding>();
        ifcBuilding.AddElement(_pipeSegment);

        return _pipeSegment;
    }

    public void Clip(IModel model, IfcNodeEntity nodeEntity, double clipLength)
    {
        if ((nodeEntity.Coordinates - ObjectMatrix3D.Translation).Length < (nodeEntity.Coordinates - ObjectMatrix3D.Translation - ObjectMatrix3D.Forward * Depth).Length)
            _extrudedArea.Position = IfcAxis.CreateAxis2Placement3D(model, ObjectMatrix3D.Forward * clipLength, ObjectMatrix3D.Forward, ObjectMatrix3D.Right);
        _extrudedArea.Depth -= clipLength;
    }

    private IfcProductDefinitionShape CreatePipeShape(IModel model)
    {
        IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
        _extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            s.Depth = Depth;
            s.Position = IfcAxis.CreateAxis2Placement3D(model, XbimVector3D.Zero, ObjectMatrix3D.Forward, ObjectMatrix3D.Right);
        });
        IfcShapeRepresentation shapeRep = IfcGeometry.CreateShapeRepresentation(model, _extrudedArea);
        IfcProductDefinitionShape productDefShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRep);

        return productDefShape;
    }

    private IfcPipeSegment CreatePipe(IModel model, IfcProductDefinitionShape productDefShape)
    {
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, ObjectMatrix3D.Translation);
        IfcPipeSegment pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = PipeEntity.GetName();
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = localPlacement;
            p.Representation = productDefShape;
        });

        return pipeSegment;
    }
}