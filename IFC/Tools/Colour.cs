using System;

namespace IFC.Tools
{
    public class Colour
    {
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }

        public Colour(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public static Colour WHITE => new Colour(255, 255, 255);
        public static Colour RED => new Colour(255, 0, 0);
        public static Colour GREEN => new Colour(0, 255, 0);
        public static Colour BLUE => new Colour(0, 0, 255);

        public static Colour FromHEX(string hex)
        {
            hex = hex.Replace("#", "");
            int rgb = Convert.ToInt32(hex, 16);
            byte red = (byte)((rgb & 0xff0000) >> 16);
            byte green = (byte)((rgb & 0xff00) >> 8);
            byte blue = (byte)(rgb & 0xff);
            return new Colour(red, green, blue);
        }

        public static Colour FromRGB(byte[] rgb)
        {
            return new Colour(rgb[0], rgb[1], rgb[2]);
        }
        
        public byte[] ToRGB()
        {
            return new byte[] { Red, Green, Blue };
        }

        public double[] ToNormal()
        {
            return new double[]
            {
                (double)Red / 255,
                (double)Green / 255,
                (double)Blue / 255,
            };
        }
    }
}