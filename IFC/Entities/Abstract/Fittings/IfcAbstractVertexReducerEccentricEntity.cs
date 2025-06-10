using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using IFC.Tools.Geometry;
using IFC.Tools.Shape;
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
    public abstract class IfcAbstractVertexReducerEccentricEntity : IfcAbstractReducerEntity
    {
        public abstract int NumSegments { get; protected set; }

        private double _displacementLength;
        private readonly StartReducerEntity _reducerEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexReducerEccentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(reducerEntity, nodeEntity, segmentEntities)
        {
            _reducerEntity = reducerEntity;

            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = CalculateForwardVector();
            XbimVector3D up = CalculateUpVector();
            _displacementLength = up.Length;
            up = up.Normalized();
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcAxisSettings axisSettings = new IfcAxisSettings(VectorExtensions.X, VectorExtensions.Y);
            XbimVector3D displacement = VectorExtensions.Y.Negated() * _displacementLength;
            IfcFacetedBrep facetedBrep = IfcVertexGeometry.CreateClippedCone(model, _PipeRadiuses[0], _PipeRadiuses[1], Length, NumSegments, axisSettings, displacement);
            
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, facetedBrep, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
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
            MovePipe(AbstractSegmentEntities[1]);
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
        
        private XbimVector3D CalculateForwardVector()
        {
            return IfcAxis.GetPipeDirectionFromNode(AbstractSegmentEntities[1], NodeEntity);
        }

        private XbimVector3D CalculateUpVector()
        {
            XbimVector3D up = AbstractSegmentEntities.Select(segment =>
            {
                XbimVector3D directionToPipe = IfcAxis.GetPipeDirectionFromNode(segment, NodeEntity);
                IfcNodeEntity startNode = NodeEntity;
                IfcNodeEntity endNode = segment.NodeEntities.First(node => node != NodeEntity);
                return endNode.ObjectMatrix3D.Translation - startNode.ObjectMatrix3D.Translation - directionToPipe * segment.Length;
            }).First(item => item != XbimVector3D.Zero);

            return up;
        }
        
        private void MovePipe(IfcAbstractSegmentEntity ifcAbstractSegmentEntity)
        {
            if ((ifcAbstractSegmentEntity.NodeEntities[0].ObjectMatrix3D.Translation - NodeEntity.ObjectMatrix3D.Translation).Length <
                (ifcAbstractSegmentEntity.NodeEntities[1].ObjectMatrix3D.Translation - NodeEntity.ObjectMatrix3D.Translation).Length)
            {
                ifcAbstractSegmentEntity.Coordinates.Value += ObjectMatrix3D.Up * _displacementLength;
            }
        }
    }
}