using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities;

public class IfcWeldedTeeEntity : IfcAbstractTeeEntity
{
    protected override IfcIdentifier Tag { get; set; } = "Welded tee";
    
    public readonly double Length;
    public readonly double Height;

    public IfcWeldedTeeEntity(StartWeldedTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes) 
        : base(teeEntity, nodeEntity, connPipes)
    {
        Length = teeEntity.GetHeaderLength();
        Height = teeEntity.GetCrotchHeight() + _branchPipes[0].Diameter / 2;
        _nodeEntity.ConnEntities.Add(this);
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        _pipeFitting = CreateTeeEntity(model, Length, Height);
        return _pipeFitting;
    }
}