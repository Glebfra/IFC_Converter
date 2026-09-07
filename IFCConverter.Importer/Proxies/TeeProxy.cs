using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    [ProxyEntity(3, typeof(TeeTopologyEntity), typeof(TeeBoundaryResolver))]
    internal sealed class TeeProxy : IFittingProxy
    {

        public TeeProxy(
            Vector<double> position,
            Vector<double> mainProjection,
            Vector<double> headProjection,
            double mainDiameter,
            double headDiameter)
        {
            Position = position;
            MainProjection = mainProjection;
            HeadProjection = headProjection;
            MainDiameter = mainDiameter;
            HeadDiameter = headDiameter;
        }

        public double HeadDiameter { get; }
        public double MainDiameter { get; }

        public Vector<double> MainProjection { get; }
        public Vector<double> HeadProjection { get; }

        public string Name { get; set; }
        public Vector<double> Position { get; set; }

        public IStartEntity ToStartEntity()
        {
            StartWeldedTeeEntity teeEntity = new StartWeldedTeeEntity
            {
                Position = Position
            };

            double headLength = HeadProjection.L2Norm();
            double mainLength = MainProjection.L2Norm();
            teeEntity.CrotchHeight.CreateFromSI(headLength - MainDiameter / 2);
            teeEntity.HeaderLength.CreateFromSI(mainLength);

            if (Name != null)
                teeEntity.Name = Name;

            return teeEntity;
        }
    }
}