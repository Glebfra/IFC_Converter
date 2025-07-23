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
    
    
    
    #else
    
    public abstract class IfcAbstractVertexReducerConcentricEntity : IfcAbstractReducerEntity
    {
        public abstract int NumSegments { get; protected set; }

        private readonly StartReducerEntity _reducerEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexReducerConcentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(reducerEntity, nodeEntity, segmentEntities)
        {
            _reducerEntity = reducerEntity;
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = CalculateForwardVector();
            XbimVector3D up = CalculateUpVector();
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcAxisSettings axisSettings = new IfcAxisSettings(XbimVector3D.Zero, VectorExtensions.X, VectorExtensions.Y);
            IfcFacetedBrep facetedBrep = IfcVertexGeometry.CreateClippedCone(
                model, _PipeRadiuses[0], _PipeRadiuses[1], Length, 
                NumSegments, axisSettings, XbimVector3D.Zero
            );
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBrep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, facetedBrep);
        
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;
                fitting.Tag = Tag;
                fitting.Name = _reducerEntity.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }

        private XbimVector3D CalculateForwardVector()
        {
            return IfcAxis.GetPipeDirectionFromNode(AbstractSegmentEntities[1], NodeEntity);
        }

        private XbimVector3D CalculateUpVector()
        {
            return AbstractSegmentEntities[1].ObjectMatrix3D.Up;
        }
    }

    #endif
}