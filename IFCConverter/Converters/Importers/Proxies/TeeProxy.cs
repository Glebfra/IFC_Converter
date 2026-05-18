using System.Collections.Generic;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Converters.Importers.Proxies
{
    internal sealed class TeeProxy : IFittingProxy
    {
        public readonly double MainDiameter;
        public readonly double HeadDiameter;
        public Vector<double> Position { get; }
        public Vector<double> MainProjection { get; }
        public Vector<double> HeadProjection { get; }

        public string? Name { get; set; }
        
        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();
        private IEnumerable<Vector<double>>? _boundary;

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

        public IStartEntity ToStartEntity()
        {
            StartWeldedTeeEntity teeEntity = new StartWeldedTeeEntity();
            teeEntity.Position = Position;

            double headLength = HeadProjection.L2Norm();
            double mainLength = MainProjection.L2Norm();
            teeEntity.CrotchHeight.CreateFromSI(headLength - MainDiameter / 2);
            teeEntity.HeaderLength.CreateFromSI(mainLength);

            if (Name != null)
                teeEntity.Name = Name;
            
            return teeEntity;
        }

        private IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            return new Vector<double>[]
            {
                Position + HeadProjection,
                Position + MainProjection / 2,
                Position - MainProjection / 2,
            };
        }
    }
}