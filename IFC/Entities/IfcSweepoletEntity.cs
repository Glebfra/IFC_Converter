#region

using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

#endregion

namespace IFC.Entities;

public class IfcSweepoletEntity : IfcAbstractTeeEntity
{
    protected override IfcIdentifier Tag { get; set; } = "Sweepolet";
    
    private double Length;
    private double Height;
    
    public IfcSweepoletEntity(StartTeeEntityProperties properties, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes) 
        : base(properties, nodeEntity, connPipes)
    {
        Length = _headPipe.Diameter;
        Height = _branchPipes[0].Diameter / 2;
        _nodeEntity.ConnEntities.Add(this);
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        _pipeFitting = CreateTeeEntity(model, Length, Height);
        return _pipeFitting;
    }
}