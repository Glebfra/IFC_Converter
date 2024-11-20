namespace IFC_Converter.Math;

public struct Vector3
{
    public double x;
    public double y;
    public double z;

    public Vector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public double Length()
    {
        return System.Math.Sqrt(x * x + y * y + z * z);
    }

    public Vector3 Normalized()
    {
        return new Vector3(x, y, z) / Length();
    }

    public static double Dot(Vector3 fisrt, Vector3 second)
    {
        return fisrt.x * second.x + fisrt.y * second.y + fisrt.z * second.z;
    }

    public static Vector3 operator +(Vector3 left, Vector3 right)
    {
        return new Vector3(
            left.x + right.x,
            left.y + right.y,
            left.z + right.z
        );
    }

    public static Vector3 operator -(Vector3 left, Vector3 right)
    {
        return new Vector3(
            left.x - right.x,
            left.y - right.y,
            left.z - right.z
        );
    }

    public static Vector3 operator *(Vector3 left, double right)
    {
        return new Vector3(
            left.x * right,
            left.y * right,
            left.z * right
        );
    }

    public static Vector3 operator /(Vector3 left, double right)
    {
        return new Vector3(
            left.x / right,
            left.y / right,
            left.z / right
        );
    }
}