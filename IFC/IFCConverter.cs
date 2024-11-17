using IFC_Converter.IFC.Entities;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.IO;

namespace IFC_Converter.IFC;

public class IFCConverter : IDisposable
{
    private readonly XbimEditorCredentials editor = new()
    {
        ApplicationDevelopersName = "Start Developer",
        ApplicationFullName = "xbim toolkit",
        ApplicationIdentifier = "xbim",
        ApplicationVersion = "4.0",
        EditorsFamilyName = "Santini Aichel",
        EditorsGivenName = "Johann Blasius",
        EditorsOrganisationName = "Independent Architecture"
    };

    private readonly IfcStore _model;
    private readonly ITransaction _transaction;
    private readonly IfcProject _project;

    public IFCConverter(string name)
    {
        _model = IfcStore.Create(editor, XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        _transaction = _model.BeginTransaction();
        _project = _model.Instances.New<IfcProject>(p => { p.Name = name; });
        _project.Initialize(ProjectUnits.SIUnitsUK);

        var lengthUnit = _model.Instances.FirstOrDefault<IfcSIUnit>(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);
        lengthUnit.Name = IfcSIUnitName.METRE;
        lengthUnit.Prefix = null;
    }

    public void AddEntity(IfcAbstractEntity entity)
    {
        entity.CreateAndAdd(_model);
    }

    public void SaveAs(string filepath)
    {
        _transaction.Commit();
        _model.SaveAs(filepath);
    }

    public void Dispose()
    {
        _transaction.Dispose();
        _model.Dispose();
    }
}