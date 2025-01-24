using IFC_Converter.IFC.Entities;
using IFC_Converter.IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.IO;

namespace IFC_Converter.IFC;

public class IFCConverter : IDisposable
{
    private readonly XbimEditorCredentials editor = new()
    {
        ApplicationDevelopersName = "Start",
        ApplicationFullName = "Start-Prof",
        ApplicationIdentifier = "Start",
        ApplicationVersion = "4.0",
        EditorsFamilyName = "Santini Aichel",
        EditorsGivenName = "Johann Blasius",
        EditorsOrganisationName = "Independent Architecture"
    };

    private readonly IfcStore _model;
    private readonly ITransaction _transaction;
    private readonly IfcProject _project;
    private readonly IfcSite _site;
    private readonly IfcBuilding _building;

    private readonly IfcSystem _pipeSystem;
    private readonly List<IfcProduct> _ifcObjects;

    public IFCConverter(string name)
    {
        _model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        _transaction = _model.BeginTransaction();
        _project = _model.Instances.New<IfcProject>(p => p.Name = name);
        _project.Initialize(ProjectUnits.SIUnitsUK);

        IfcSIUnit lengthUnit = _model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
        lengthUnit.Name = IfcSIUnitName.METRE;
        lengthUnit.Prefix = null;

        _site = _model.Instances.New<IfcSite>(ifcSite =>
        {
            ifcSite.Name = "Site";
            ifcSite.CompositionType = IfcElementCompositionEnum.ELEMENT;
            ifcSite.ObjectPlacement = IfcAxis.CreateLocalPlacement(_model, XbimVector3D.Zero);
        });
        _project.AddSite(_site);

        _building = _model.Instances.New<IfcBuilding>(ifcBuilding =>
        {
            ifcBuilding.Name = "Building";
            ifcBuilding.CompositionType = IfcElementCompositionEnum.ELEMENT;
            ifcBuilding.ObjectPlacement = IfcAxis.CreateLocalPlacement(_model, XbimVector3D.Zero);
        });
        _site.AddBuilding(_building);

        _ifcObjects = new List<IfcProduct>();
    }

    public IfcProduct AddEntity(IfcAbstractEntity entity)
    {
        IfcProduct @object = entity.CreateAndAdd(_model);
        _ifcObjects.Add(@object);
        _building.AddElement(@object);
        return @object;
    }

    public IfcProduct[] AddEntities(IfcAbstractEntity[] entities)
    {
        return entities.Select(entity => AddEntity(entity)).ToArray();
    }

    public void GroupObjects(string groupName)
    {
        var pipeSystem = _model.Instances.New<IfcSystem>(sys => { sys.Name = groupName; });

        IfcRelAssignsToGroup relAssignsToGroup = _model.Instances.New<IfcRelAssignsToGroup>(rel =>
        {
            rel.RelatingGroup = pipeSystem;
            rel.RelatedObjects.AddRange(_ifcObjects);
        });
    }

    public void SaveAs(string filepath)
    {
        _transaction.Commit();
        _model.SaveAs(filepath, StorageType.Ifc);
    }

    public void Dispose()
    {
        _transaction.Dispose();
        _model.Dispose();
    }
}