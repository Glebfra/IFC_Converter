using System;
using Ifc.Interfaces;

namespace Ifc.API
{
    public readonly struct Color : IColor
    {
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }

        public static Color WHITE => new(255, 255, 255);
        public static Color RED => new(255, 0, 0);
        public static Color GREEN => new(0, 255, 0);
        public static Color BLUE => new(0, 0, 255);

        public Color(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public static Color FromHEX(string hex)
        {
            hex = hex.Replace("#", "");
            int rgb = Convert.ToInt32(hex, 16);
            byte red = (byte)((rgb & 0xff0000) >> 16);
            byte green = (byte)((rgb & 0xff00) >> 8);
            byte blue = (byte)(rgb & 0xff);
            return new Color(red, green, blue);
        }

        public static Color FromRGB(byte[] rgb)
        {
            return new Color(rgb[0], rgb[1], rgb[2]);
        }

        public byte[] ToRGB()
        {
            return new[] { Red, Green, Blue };
        }

        public double[] ToNormal()
        {
            return new[] { (double)Red / 255, (double)Green / 255, (double)Blue / 255 };
        }
    }
}