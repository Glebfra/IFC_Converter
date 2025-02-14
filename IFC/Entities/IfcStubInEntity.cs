#region

using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

#endregion

namespace IFC.Entities;

public class IfcStubInEntity : IfcAbstractTeeEntity
{
    protected override IfcIdentifier Tag { get; set; } = "Stub-in";
    
    public readonly double Length;
    public readonly double Height;
    
    public IfcStubInEntity(StartStubInEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes) 
        : base(teeEntity, nodeEntity, connPipes)
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