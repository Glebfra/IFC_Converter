using IFC.Entities.Abstract;
using IFC.Tools;
using Start.API;
using Start.Entities;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
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
        
        public IfcBendEntity(StartBendEntity bendEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities)
            : base(bendEntity, ifcNodeEntity, ifcAbstractSegmentEntities)
        {
            _bendEntity = bendEntity;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCircle circle = IfcGeometry.CreateCircle(model, _BendRadius, XbimVector3D.Zero, new XbimVector3D(0, 1, 0), new XbimVector3D(1, 0, 0));
            IfcTrimmedCurve bendCurve = IfcGeometry.CreateTrimmedCurve(model, circle, -Angle, 0);
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateSweptDiskSolid(model, bendCurve, _PipeRadius);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, pipeBend);

            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
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