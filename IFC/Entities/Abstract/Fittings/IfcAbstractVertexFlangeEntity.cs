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

    public abstract class IfcAbstractVertexFlangeEntity : IfcAbstractFittingEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double[] Radiuses { get; protected set; }

        private readonly StartArmatureEntity _armatureEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexFlangeEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(armatureEntity, nodeEntity, segmentEntities)
        {
            _armatureEntity = armatureEntity;
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
                IfcVertexGeometry.CreateCircle(model, Radiuses[0], displacements[0].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[0] * 1.1, displacements[1].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[0] * 1.5, displacements[1].Negated(), NumSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[0] * 1.5, displacements[2].Negated(), NumSegments),
                
                IfcVertexGeometry.CreateCircle(model, Radiuses[1], displacements[0], NumSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[1] * 1.1, displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[1] * 1.5, displacements[1], NumSegments),
                IfcVertexGeometry.CreateCircle(model, Radiuses[1] * 1.5, displacements[2], NumSegments),
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
            ColourEntity(model, facetedBreps);

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
            ClipPipes();

            return _pipeFitting;
        }

        private void ClipPipes()
        {
            foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in AbstractSegmentEntities)
            {
                ifcAbstractSegmentEntity.Clip(NodeEntity, Length / 2);
            }
        }
    }

    #endif
}