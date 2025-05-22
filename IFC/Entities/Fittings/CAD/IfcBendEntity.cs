using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcBendEntity : IfcAbstractBendEntity
    {
        private StartBendEntity _bendEntity;
        private IfcPipeFitting _pipeFitting;
        
        public IfcBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(bendEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _bendEntity = bendEntity;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateCircularBend(
                model, _PipeRadius, _BendRadius, Angle,
                XbimVector3D.Zero, VectorExtensions.Forward, VectorExtensions.Right
            );
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, pipeBend);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, pipeBend);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _bendEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            AddProperties(model, _pipeFitting);
            ClipConnectedPipes();

            return _pipeFitting;
        }
    }
}