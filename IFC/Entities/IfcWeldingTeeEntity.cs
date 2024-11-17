using IFC_Converter.Math;
using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public class IfcWeldingTeeEntity : IfcAbstractEntity
{
    private StartWeldingTeeEntity _teeEntity;
    private StartPipeEntity[] _connPipes;
    private StartNodeEntity _nodeEntity;

    public Vector3 Coordinates;

    public IfcWeldingTeeEntity(StartWeldingTeeEntity teeEntity, StartNodeEntity nodeEntity, StartPipeEntity[] connPipes)
    {
        _teeEntity = teeEntity;
        _nodeEntity = nodeEntity;
        _connPipes = connPipes;

        Coordinates = _nodeEntity.GetCoordinates();
    }

    public IfcProductDefinitionShape[] CreateAndAddWeldingTee(IModel model)
    {
        IfcProductDefinitionShape[] productDefinitionShapes = new IfcProductDefinitionShape[_connPipes.Length];

        int i = 0;
        foreach (var pipeEntity in _connPipes)
        {
            Vector3 pipeStartCoordinates = pipeEntity.GetCoordinates();
            Vector3 pipeDirection = pipeEntity.GetDirection();
            Vector3 pipeEndCoordinates = pipeStartCoordinates + pipeDirection;
            Vector3 weldedTeeBranchDirection = (pipeStartCoordinates - Coordinates).Length() < (pipeEndCoordinates - Coordinates).Length() ? pipeDirection : pipeDirection * -1;

            IfcLocalPlacement localStartPlacement = CreateLocalPlacementAndDirection(model, Coordinates, weldedTeeBranchDirection);

            IfcProductDefinitionShape productDefShape = CreatePipeShape(model, pipeEntity);
            IfcPipeSegment? pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
            {
                p.Name = _teeEntity.GetName();
                p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                p.ObjectPlacement = localStartPlacement;
                p.Representation = productDefShape;
            });
            AddProperties(model, pipeSegment, _teeEntity);

            productDefinitionShapes[i++] = productDefShape;
        }

        return productDefinitionShapes;
    }

    private IfcProductDefinitionShape CreatePipeShape(IModel model, StartPipeEntity startPipeEntity)
    {
        IfcCircleProfileDef? profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = startPipeEntity.GetOutsideDiameter() / 2;
        });

        IfcExtrudedAreaSolid? extrudedSolid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1));
            s.Depth = _teeEntity.GetBranchHeight();
        });

        IfcShapeRepresentation? shapeRep = model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(extrudedSolid);
        });

        IfcProductDefinitionShape? productDefShape = model.Instances.New<IfcProductDefinitionShape>(repr => { repr.Representations.Add(shapeRep); });

        return productDefShape;
    }
}