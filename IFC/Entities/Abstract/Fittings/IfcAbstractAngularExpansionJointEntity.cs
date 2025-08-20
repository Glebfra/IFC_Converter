using System.Collections.Generic;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexAngularExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        public abstract double Angle { get; }
        public abstract double Diameter { get; }
        public abstract int NumSegments { get; }
        
        protected IfcAbstractVertexAngularExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            ClipPipes();
            return pipeFitting;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.CONNECTOR;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);
            
            return pipeFitting;
        }

        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimMatrix3D My = MatrixExtensions.My(Angle);
            
            XbimVector3D firstExtrudeDirection = VectorExtensions.Forward.Negated();
            XbimVector3D secondExtrudeDirection = XbimVector3D.Multiply(firstExtrudeDirection, My).Negated();
            
            XbimVector3D firstProfileRefDirection = VectorExtensions.Right;
            XbimVector3D secondProfileRefDirection = XbimVector3D.Multiply(firstProfileRefDirection, My).Negated();

            IfcRepresentationItem[] representationItems = new IfcRepresentationItem[]
            {
                IfcVertexGeometry.CreateSphere(model, Diameter * 0.75, XbimVector3D.Zero, NumSegments, VectorExtensions.X, VectorExtensions.Y)
            };

            return representationItems;
        }

        private IfcExtrudedAreaSolid CreateBranch(IModel model, XbimVector3D extrudeDirection, XbimVector3D refDirection)
        {
            IfcDirection firstExtrudedDirection = IfcAxis.CreateDirection(model, extrudeDirection);
            IfcCircleProfileDef firstProfileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero, refDirection);
            return CreateExtrudedArea(model, firstProfileDef, firstExtrudedDirection, Length / 2);
        }
        
        private IfcExtrudedAreaSolid CreateExtrudedArea(IModel model, IfcProfileDef profileDef, IfcDirection direction, double length)
        {
            return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
            {
                solid.Depth = length;
                solid.ExtrudedDirection = direction;
                solid.SweptArea = profileDef;
            });
        }
    }
}