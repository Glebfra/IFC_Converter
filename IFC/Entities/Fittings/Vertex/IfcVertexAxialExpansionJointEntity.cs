using System;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
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
    public sealed class IfcVertexAxialExpansionJointEntity : IfcAbstractAxialExpansionJointEntity
    {
        private readonly int _numSegments;
        private readonly double _angleStep;

        private StartAxialExpansionJointEntity _startAxialExpansionJointEntity;
        private IfcPipeFitting _pipeFitting;
        
        public IfcVertexAxialExpansionJointEntity(StartAxialExpansionJointEntity startAxialExpansionJointEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(startAxialExpansionJointEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _numSegments = numSegments;
            _angleStep = 2 * Math.PI / _numSegments;
            _startAxialExpansionJointEntity = startAxialExpansionJointEntity;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * Length * VectorExtensions.Z,
                XbimVector3D.Zero,
            };

            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, _Radiuses[0], displacements[0].Negated(), _numSegments),
                IfcVertexGeometry.CreateCircle(model, _Radiuses[0], displacements[1], _numSegments),
                IfcVertexGeometry.CreateCircle(model, _Radiuses[1], displacements[1], _numSegments),
                IfcVertexGeometry.CreateCircle(model, _Radiuses[1], displacements[0], _numSegments),
            };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcVertexGeometry.CreateClippedCone(model, circles[0], circles[1]),
                IfcVertexGeometry.CreateClippedCone(model, circles[1], circles[2]),
                IfcVertexGeometry.CreateClippedCone(model, circles[2], circles[3]),
            };
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, facetedBreps);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _startAxialExpansionJointEntity.Name;
            });
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }
}