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
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Fittings
{
    #if NEW
    
    public abstract class IfcAbstractAxialExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        public abstract double Diameter { get; }
        public abstract int NumSegments { get; }
        
        protected IfcAbstractAxialExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            return pipeFitting;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            
            IfcCircleProfileDef[] profileDefs = new IfcCircleProfileDef[2];
            double[] radiuses = new double[] { Diameter / 2 * 1.1, Diameter / 2 * 0.9 };
            profileDefs[0] = IfcGeometry.CreateCircleProfileDef(model, radiuses[0], XbimVector3D.Zero);
            profileDefs[1] = IfcGeometry.CreateCircleProfileDef(model, radiuses[1], XbimVector3D.Zero);

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[]
            {
                IfcGeometry.CreateExtrudedAreaSolid(model, profileDefs[0], Length / 2, XbimVector3D.Zero, VectorExtensions.Forward.Negated(), VectorExtensions.Right.Negated()),
                IfcGeometry.CreateExtrudedAreaSolid(model, profileDefs[1], Length / 2, XbimVector3D.Zero, VectorExtensions.Forward, VectorExtensions.Right)
            };
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }
    }
    
    #else
    
    public abstract class IfcAbstractAxialExpansionJointEntity : IfcAbstractExpansionJoint
    {
        public abstract double PipeDiameter { get; protected set; }
        
        private readonly StartAxialExpansionJointEntity _axialExpansionJoint;
        private IfcPipeFitting? _pipeFitting;
        
        public IfcAbstractAxialExpansionJointEntity(StartAxialExpansionJointEntity axialExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(axialExpansionJoint, nodeEntity, segmentEntities)
        {
            _axialExpansionJoint = axialExpansionJoint;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            
            IfcCircleProfileDef[] profileDefs = new IfcCircleProfileDef[2];
            double[] radiuses = new double[] { PipeDiameter / 2 * 1.1, PipeDiameter / 2 * 0.9 };
            profileDefs[0] = IfcGeometry.CreateCircleProfileDef(model, radiuses[0], XbimVector3D.Zero);
            profileDefs[1] = IfcGeometry.CreateCircleProfileDef(model, radiuses[1], XbimVector3D.Zero);
            
            IfcExtrudedAreaSolid[] extrudedAreaSolids = new IfcExtrudedAreaSolid[2];
            extrudedAreaSolids[0] = IfcGeometry.CreateExtrudedAreaSolid(model, profileDefs[0], Length / 2, XbimVector3D.Zero, VectorExtensions.Forward.Negated(), VectorExtensions.Right.Negated());
            extrudedAreaSolids[1] = IfcGeometry.CreateExtrudedAreaSolid(model, profileDefs[1], Length / 2, XbimVector3D.Zero, VectorExtensions.Forward, VectorExtensions.Right);

            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, extrudedAreaSolids);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, extrudedAreaSolids);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
                fitting.Representation = shape;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;
                fitting.Tag = Tag;
                fitting.Name = _axialExpansionJoint.Name;
            });
            
            ClipPipes();
            AddProperties(model, _pipeFitting);

            return _pipeFitting;
        }
    }

    #endif
}