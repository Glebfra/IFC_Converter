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
        IfcPipeSegment[] ifcPipeSegments = new IfcPipeSegment[_connPipes.Length];
        IIfcSolidModel? baseGeometry = null;

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

            IfcProductDefinitionShape productDefShape = CreatePipeShape(model, pipeEntity);
            IfcPipeSegment? pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
            {
                p.Name = _teeEntity.GetName();
                p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                p.ObjectPlacement = localStartPlacement;
                p.Representation = productDefShape;
            });
            AddProperties(model, pipeSegment, _teeEntity);

            ifcPipeSegments[i++] = pipeSegment;
        }

        IfcGroup teePipesGroup = model.Instances.New<IfcGroup>(group => group.Name = _teeEntity.GetName());
        IfcRelAssignsToGroup groupAssignment = model.Instances.New<IfcRelAssignsToGroup>(rel =>
        {
            rel.RelatedObjects.AddRange(ifcPipeSegments);
            rel.RelatingGroup = teePipesGroup;
        });
    }

    private IfcProductDefinitionShape CreatePipeShape(IModel model, StartPipeEntity startPipeEntity)
    {
        IfcCircleProfileDef? profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = startPipeEntity.GetOutsideDiameter() * 1.1 / 2;
        });

        IfcExtrudedAreaSolid? extrudedSolid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1));
            s.Depth = _teeEntity.GetBranchHeight() / 2;
        });

        IfcShapeRepresentation? shapeRep = model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(extrudedSolid);
        });

        IfcProductDefinitionShape? productDefShape =
            model.Instances.New<IfcProductDefinitionShape>(repr => { repr.Representations.Add(shapeRep); });

        return productDefShape;
    }
}