using System;
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
    #if NEW 
    
    public abstract class IfcAbstractCadBendEntity : IfcAbstractBendEntity
    {
        protected IfcAbstractCadBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            return pipeFitting;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
            
            XbimVector3D bendDisplacement = BendRadius / Math.Cos(Angle / 2) * (VectorExtensions.Forward + VectorExtensions.Right).Normalized().Negated();
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateCircularBend(
                model, PipeRadius, BendRadius, Angle,
                bendDisplacement, VectorExtensions.Forward, VectorExtensions.Right
            );
            AddShapeRepresentation(model, pipeFitting, pipeBend);

            return pipeFitting;
        }
    }
    
    #else
    
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
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            XbimVector3D bendDisplacement = BendRadius / Math.Cos(Angle / 2) * (VectorExtensions.Forward + VectorExtensions.Right).Normalized().Negated();
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateCircularBend(
                model, PipeRadius, BendRadius, Angle,
                bendDisplacement, VectorExtensions.Forward, VectorExtensions.Right
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

    #endif
}