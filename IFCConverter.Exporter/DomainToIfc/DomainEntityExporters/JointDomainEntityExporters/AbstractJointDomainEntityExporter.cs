using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal abstract class AbstractJointDomainEntityExporter : IJointDomainEntityExporter
    {
        public abstract bool CanExport(Joint joint);

        public void Export(Joint joint, IModel model, ExportContext context)
        {
            IIfcGeometry geometry = CreateGeometry(joint, model);
            geometry.AssignColor(Color.FromHEX(joint.Metadata.Color));

            Matrix<double> placement = MatrixExtensions.CreateTransition(joint.Position);
            IIfcPipeFittingBuilder<IIfcPipeFitting> builder =
                new IfcPipeFittingBuilder<IfcPipeFitting>(joint.Metadata.Name, joint.Metadata.Type, IfcPipeFittingTypeEnum.CONNECTOR);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(joint, instance);
        }

        abstract protected IIfcGeometry CreateGeometry(Joint joint, IModel model);
    }
}