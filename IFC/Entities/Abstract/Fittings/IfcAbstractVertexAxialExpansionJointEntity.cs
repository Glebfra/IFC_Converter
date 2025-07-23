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

namespace IFC.Entities.Abstract.Fittings
{
    #if NEW
    
    
    
    #else

    public abstract class IfcAbstractVertexAxialExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract double PipeDiameter { get; protected set; }
        public abstract int NumSegments { get; protected set; }

        private readonly StartAxialExpansionJointEntity _expansionJoint;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexAxialExpansionJointEntity(StartAxialExpansionJointEntity expansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(expansionJoint, nodeEntity, segmentEntities)
        {
            _expansionJoint = expansionJoint;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            XbimVector3D[] displacements = new XbimVector3D[]
            {
                0.5 * Length * VectorExtensions.Z,
                XbimVector3D.Zero,
            };
            
            double[] radiuses = new double[] { PipeDiameter / 2 * 1.1, PipeDiameter / 2 * 0.9 };
            IfcCartesianPoint[][] circles = new IfcCartesianPoint[][]
            {
                IfcVertexGeometry.CreateCircle(model, radiuses[0], displacements[0].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[0], displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1], displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, radiuses[1], displacements[0], NumSegments),
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
                fitting.Name = _expansionJoint.Name;
            });
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }

    #endif
}