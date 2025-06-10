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
    public abstract class IfcAbstractVertexBallExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double Radius { get; protected set; }
        
        private readonly StartBallExpansionJointEntity _ballExpansionJoint;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexBallExpansionJointEntity(StartBallExpansionJointEntity ballExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(ballExpansionJoint, nodeEntity, segmentEntities)
        {
            _ballExpansionJoint = ballExpansionJoint;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcFacetedBrep brep = IfcVertexGeometry.CreateSphere(model, Radius, XbimVector3D.Zero, NumSegments, VectorExtensions.X, VectorExtensions.Y);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, brep, IfcRepresentationType.Brep, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, brep);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _ballExpansionJoint.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }
}