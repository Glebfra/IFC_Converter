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
    public abstract class IfcAbstractVertexTorsionExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Radius { get; protected set; }

        private readonly StartTorsionExpansionJointEntity _torsionExpansion;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexTorsionExpansionJointEntity(StartTorsionExpansionJointEntity torsionExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(torsionExpansion, nodeEntity, segmentEntities)
        {
            _torsionExpansion = torsionExpansion;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            double displacementStep = Length / 4;
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                VectorExtensions.Z * 2 * displacementStep,
                VectorExtensions.Z * 1 * displacementStep,
            };
            double[] radiuses = { Radius, Radius * 1.2, Radius * 2 };

            IfcFacetedBrep[] facetedBreps = new IfcFacetedBrep[]
            {
                IfcVertexGeometry.CreateClippedCone(
                    model, radiuses[1], radiuses[0], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[0], VectorExtensions.X, VectorExtensions.Y)
                ),
                IfcVertexGeometry.CreateClippedCone(
                    model, radiuses[2], radiuses[2], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[1], VectorExtensions.X, VectorExtensions.Y)
                ),
                IfcVertexGeometry.CreateClippedCone(
                    model, radiuses[2], radiuses[2], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[1].Negated(), VectorExtensions.X, VectorExtensions.Y)
                ),
                IfcVertexGeometry.CreateClippedCone(
                    model, radiuses[0], radiuses[1], displacementStep, 
                    NumSegments, new IfcAxisSettings(displacements[0].Negated(), VectorExtensions.X, VectorExtensions.Y)
                ),
            };
            ColourEntity(model, facetedBreps);
            
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBreps);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Name = _torsionExpansion.Name;
                fitting.Tag = Tag;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
            });

            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }
}