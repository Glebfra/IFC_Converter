using System;
using System.Linq;
using IFC.Entities.Abstract;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Fittings.Vertex
{
    public sealed class IfcVertexFlangeEntity : IfcAbstractFittingEntity
    {
        public readonly double Length;
        public readonly double[] Radiuses;
        
        private readonly int _numSegments;
        private readonly double _angleStep;
        private readonly StartArmatureEntity _armatureEntity;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexFlangeEntity(StartArmatureEntity armatureEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments)
            : base(armatureEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            
            _armatureEntity = armatureEntity;
            Length = _armatureEntity.Length;
            Radiuses = abstractSegmentEntities.Select(entity => entity.Diameter / 2).ToArray();
        }
    
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * Length * VectorExtensions.Z,
                0.3 * Length * VectorExtensions.Z,
                0.1 * Length * VectorExtensions.Z,
            };

            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, Radiuses[0], displacements[0].Negated(), _numSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[0] * 1.1, displacements[1].Negated(), _numSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[0] * 1.5, displacements[1].Negated(), _numSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[0] * 1.5, displacements[2].Negated(), _numSegments),
                
                IfcVertexGeometry.CreateCircle(model, Radiuses[1], displacements[0], _numSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[1] * 1.1, displacements[1], _numSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[1] * 1.5, displacements[1], _numSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[1] * 1.5, displacements[2], _numSegments),
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]),
                IfcVertexGeometry.CreateClippedCone(model, circles[1], circles[2]),
                IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]),
                IfcVertexGeometry.CreateClippedCone(model, circles[4], circles[5]),
                IfcVertexGeometry.CreateClippedCone(model, circles[5], circles[6]),
                IfcVertexGeometry.CreateClippedCone(model, circles[6], circles[7]),
            };

            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Name = _armatureEntity.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
            });

            AddProperties(model, _pipeFitting);
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, 0.5 * Length);
            }

            return _pipeFitting;
        }
    }
}