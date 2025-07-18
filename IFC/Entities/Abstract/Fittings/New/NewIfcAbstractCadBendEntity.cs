using System;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class NewIfcAbstractCadBendEntity : NewIfcAbstractBendEntity
    {
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            return pipeFitting;
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
            
            XbimVector3D bendDisplacement = BendRadius / Math.Cos(Angle / 2) * (VectorExtensions.Forward + VectorExtensions.Right).Normalized().Negated();
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateCircularBend(
                model, PipeRadius, BendRadius, Angle,
                bendDisplacement, VectorExtensions.Forward, VectorExtensions.Right
            );
            AddShapeRepresentation(model, pipeFitting, pipeBend);

            return pipeFitting;
        }
    }
}