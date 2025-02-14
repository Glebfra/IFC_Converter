using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.IO;

namespace IFC;

public class IFCProject : IDisposable
{
    private ITransaction _transaction;

    private readonly IfcStore _model;
    private readonly IfcBuilding _building;
    
    private readonly List<IfcProduct> _ifcObjects;

    public static IFCProject CreateProject(string name)
    {
        XbimEditorCredentials editor = new()
        {
            ApplicationDevelopersName = "Start",
            ApplicationFullName = "Start-Prof",
            ApplicationIdentifier = "Start",
            ApplicationVersion = "4.0",
            EditorsFamilyName = "Santini Aichel",
            EditorsGivenName = "Johann Blasius",
            EditorsOrganisationName = "Independent Architecture"
        };
        
        IfcStore model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        ITransaction transaction = model.BeginTransaction("Model creation");
        IfcProject project = model.Instances.New<IfcProject>(p => p.Name = name);
        project.Initialize(ProjectUnits.SIUnitsUK);
        
        IfcSIUnit lengthUnit = model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
        lengthUnit.Name = IfcSIUnitName.METRE;
        lengthUnit.Prefix = null;
        
        IfcCartesianPoint point = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
        IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);
        
        IfcSite site = model.Instances.New<IfcSite>(ifcSite =>
        {
            ifcSite.Name = "Site";
            ifcSite.CompositionType = IfcElementCompositionEnum.ELEMENT;
            ifcSite.ObjectPlacement = localPlacement;
        });
        project.AddSite(site);

        IfcBuilding building = model.Instances.New<IfcBuilding>(ifcBuilding =>
        {
            ifcBuilding.Name = "Building";
            ifcBuilding.CompositionType = IfcElementCompositionEnum.ELEMENT;
            ifcBuilding.ObjectPlacement = localPlacement;
        });
        site.AddBuilding(building);
        transaction.Commit();

        return new IFCProject(model);
    }

    public IFCProject(IfcStore model)
    {
        _model = model;
        _building = _model.Instances.FirstOrDefault<IfcBuilding>();
        _transaction = _model.BeginTransaction("Objects adding");

        _ifcObjects = new List<IfcProduct>();
    }

    public IfcProduct AddEntity(IfcAbstractEntity entity)
    {
        IfcProduct @object = entity.CreateAndAdd(_model);
        _ifcObjects.Add(@object);
        _building.AddElement(@object);
        return @object;
    }

    public IfcProduct[] AddEntities(IEnumerable<IfcAbstractEntity> entities)
    {
        return entities.Select(entity => AddEntity(entity)).ToArray();
    }

    public void GroupObjects(string groupName)
    {
        IfcSystem pipeSystem = _model.Instances.New<IfcSystem>(sys => { sys.Name = groupName; });

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