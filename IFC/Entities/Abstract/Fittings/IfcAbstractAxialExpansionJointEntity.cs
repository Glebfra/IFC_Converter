using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractAxialExpansionJointEntity : IfcAbstractExpansionJointEntity
    {
        public abstract double Diameter { get; }
        public abstract int NumSegments { get; }
        
        protected IfcAbstractAxialExpansionJointEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

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
}