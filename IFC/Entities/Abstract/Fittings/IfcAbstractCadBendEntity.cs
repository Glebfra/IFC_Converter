using IFC.Entities.Abstract.Segments;
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

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractCadBendEntity : IfcAbstractBendEntity
    {
        private readonly StartBendEntity _bendEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractCadBendEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            _bendEntity = bendEntity;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            ObjectMatrix3D = ObjectMatrix3D.Translate(CalculateCircleCenter());
            
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateCircularBend(
                model, PipeRadius, BendRadius, Angle,
                XbimVector3D.Zero, VectorExtensions.Forward, VectorExtensions.Right
            );
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, pipeBend, IfcRepresentationType.AdvancedSweptSolid, IfcRepresentationIdentifier.Body);
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