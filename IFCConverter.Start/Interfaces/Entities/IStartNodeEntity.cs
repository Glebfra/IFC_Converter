using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.Interfaces
{
    public interface IStartNodeEntity : IStartEntity
    {
        string Description { get; set; }

        IStartValueProperty<double> XCoord { get; set; }
        IStartValueProperty<double> YCoord { get; set; }
        IStartValueProperty<double> ZCoord { get; set; }

        FixedVector<Dim3> Position { get; set; }
    }
}