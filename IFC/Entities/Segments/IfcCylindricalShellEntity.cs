using IFC.Entities.Abstract;
using IFC.Extensions;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Segments
{
    public sealed class IfcCylindricalShellEntity : IfcAbstractSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override XbimVector3D Direction { get; protected set; }
        public override double OuterDiameter { get; protected set; }

        private StartPipeEntity _startPipeEntity;
        private IfcPipeSegment _pipeSegment;
        
        public IfcCylindricalShellEntity(StartPipeEntity startPipeEntity, IfcNodeEntity[] ifcNodeEntities) 
            : base(startPipeEntity, ifcNodeEntities)
        {
            _startPipeEntity = startPipeEntity;
            Coordinates = ifcNodeEntities[0].ObjectMatrix3D.Translation;
            Direction = ifcNodeEntities[1].ObjectMatrix3D.Translation - Coordinates;
            Length = Direction.Length;

            XbimVector3D forward = Direction.Normalized();
            ObjectMatrix3D = MatrixExtensions.CreateWorld(Coordinates, forward);

            OuterDiameter = _startPipeEntity.Diameter.SIProperty;
            OuterSurfaceArea = MathExtensions.CalculateCylinderArea(OuterDiameter / 2, Length);
            
            _OnLengthChanged += () => MathExtensions.CalculateCylinderArea(OuterDiameter / 2, Length);
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _startPipeEntity.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }
}