namespace IFCConverter.IFC.Interfaces
{
    public interface IColor
    {
        byte Red { get; }
        byte Green { get; }
        byte Blue { get; }

        byte[] ToRGB();
        double[] ToNormal();
    }
}