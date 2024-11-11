using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using IFC_Converter.Math;

namespace IFC_Converter.IFC.Entities;

public class IfcPipeEntity
{
    private readonly StartPipeEntity _pipeEntity;
    
    public Vector3 StartCoordinates;
    public Vector3 EndCoordinates;
    public Vector3 Direction;
    public double Diameter;

    public IfcPipeEntity(StartPipeEntity pipeEntity)
    {
        _pipeEntity = pipeEntity;
        StartCoordinates = new Vector3(_pipeEntity.GetXCoord(), _pipeEntity.GetYCoord(), _pipeEntity.GetZCoord());
        Direction = new Vector3(_pipeEntity.GetProjectionAlongOXAxis(), _pipeEntity.GetProjectionAlongOYAxis(), _pipeEntity.GetProjectionAlongOZAxis());
        EndCoordinates = StartCoordinates + Direction;
        Diameter = _pipeEntity.GetOutsideDiameter();
    }

    public IfcPipeSegment CreateAndAddPipe(IModel model)
    {
        IfcLocalPlacement? localPlacement = model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>(pos =>
            {
                pos.Location = model.Instances.New<IfcCartesianPoint>(pt => pt.SetXYZ(
                    StartCoordinates.x, StartCoordinates.y, StartCoordinates.z
                ));
                pos.Axis = model.Instances.New<IfcDirection>(dir => dir.SetXYZ(
                    Direction.x, Direction.y, Direction.z
                ));
            });
        });

        IfcPipeSegment? pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = "Example Pipe";
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
        });
        pipeSegment.ObjectPlacement = localPlacement;

        IfcCircleProfileDef? profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = Diameter / 2;
        });

        IfcExtrudedAreaSolid? extrudedSolid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1));
            s.Depth = Direction.Length();
        });

        IfcShapeRepresentation? shapeRep = model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(extrudedSolid);
        });

        IfcProductDefinitionShape? productDefShape = model.Instances.New<IfcProductDefinitionShape>();
        productDefShape.Representations.Add(shapeRep);
        pipeSegment.Representation = productDefShape;

        model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(pipeSegment);
            rel.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pipe properties";
                foreach (var kvp in _pipeEntity.GetData())
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(prop =>
                    {
                        prop.Name = kvp.Key;
                        prop.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
        });

        return pipeSegment;
    }
}