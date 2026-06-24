using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    [ProxyEntity(3, typeof(TeeConnectionAugmenter), typeof(TeeBoundaryResolver))]
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

        public string? Name { get; set; }
        public Vector<double> Position { get; set; }

        public IStartEntity ToStartEntity()
        {
            StartWeldedTeeEntity teeEntity = new();
            teeEntity.Position = Position;

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