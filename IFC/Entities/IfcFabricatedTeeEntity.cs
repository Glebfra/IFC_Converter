using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFC_Converter.IFC.Entities;

public class IfcFabricatedTeeEntity : IfcAbstractTeeEntity
{
    public sealed override double Length { get; protected init; }
    public sealed override double Height { get; protected init; }
    public sealed override string Name { get; protected init; }
    
    public IfcFabricatedTeeEntity(StartFabricatedTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] connPipes) 
        : base(teeEntity, nodeEntity, connPipes)
    {
        Length = teeEntity.GetHeaderLength();
        Height = teeEntity.GetBranchHeight() + _branchPipes[0].Diameter / 2;
        Name = teeEntity.GetName();
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        _pipeFitting = CreateTeeEntity(model, Length, Height);
        return _pipeFitting;
    }
}