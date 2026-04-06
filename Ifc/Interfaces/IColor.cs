namespace Ifc.Interfaces
{
    public interface IColor
    {
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }

        public byte[] ToRGB();
        public double[] ToNormal();
    }
}