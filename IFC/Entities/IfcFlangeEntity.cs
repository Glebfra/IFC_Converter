using IFC.Entities.Abstract;
using IFC.Tools;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFC.Entities;

public class IfcFlangeEntity : IfcAbstractEntity
{
    protected override IfcIdentifier Tag { get; set; } = "Flange";
    
    private const int _numSegments = 32;
    private const double _angleStep = 2 * Math.PI / _numSegments;
    
    private readonly StartFlangeEntity _startFlangeEntity;
    private readonly IfcNodeEntity _ifcNodeEntity;
    private readonly IfcPipeEntity[] _ifcPipeEntities;
    
    private IfcPipeFitting? _pipeFitting;

    public readonly double Length;
    public readonly double Diameter;
    
    public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }

    public IfcFlangeEntity(StartFlangeEntity startFlangeEntity, IfcNodeEntity ifcNodeEntity, IfcPipeEntity[] ifcPipeEntities)
    {
        _startFlangeEntity = startFlangeEntity;
        _ifcNodeEntity = ifcNodeEntity;
        _ifcPipeEntities = ifcPipeEntities;
        
        XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
        XbimVector3D forward = _ifcPipeEntities[0].ObjectMatrix3D.Forward;
        XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
        if (forward == WorldUp || forward == -1 * WorldUp) 
            WorldUp = new XbimVector3D(0, 1, 0);
        XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
        
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);

        Length = _startFlangeEntity.GetLength();
        Diameter = _startFlangeEntity.GetOutsideDiameter();
    }
    
    public override IfcProduct CreateAndAdd(IModel model)
    {
        IfcCartesianPoint point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        IfcDirection forwardDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
        IfcDirection rightDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);
        IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point, forwardDirection, rightDirection);
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);
        
        _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.Name = _startFlangeEntity.GetName();
            fitting.Tag = Tag;
            fitting.ObjectPlacement = localPlacement;
        });
        AddProperties(model, _pipeFitting);
        
        return _pipeFitting;
    }

    protected override void AddProperties(IModel model, IfcProduct product)
    {
        base.AddProperties(model, product);
        
        #region Pset_PipeFittingTypeStart
        
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(product);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeStart";
                foreach (var kvp in _startFlangeEntity.GetData())
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = kvp.Key;
                        value.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
        });

        #endregion
    }
}