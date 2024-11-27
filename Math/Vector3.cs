namespace IFC_Converter.Math;

public struct Vector3
{
    public double x;
    public double y;
    public double z;

    public Vector3 XYZ => new(x, y, z);
    public Vector3 YZX => new(y, z, x);
    public Vector3 ZXY => new(z, x, y);
    
    public double Length => System.Math.Sqrt(Dot(this, this));
    public Vector3 Normalized => new Vector3(x, y, z) / Length;

    public static Vector3 Up => new Vector3(0, 0, 1);
    public static Vector3 Right => new Vector3(0, 1, 0);
    public static Vector3 Forward => new Vector3(1, 0, 0);
    
    public static Vector3 Zero => new Vector3();
    public static Vector3 One => new Vector3(1, 1, 1);

    public Vector3(double x=0, double y=0, double z=0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3 MatMul(Vector3 first, Vector3 second)
    {
        return new Vector3(
            first.y * second.z - first.z * second.y,
            first.z * second.x - first.x * second.z,
            first.y * second.z - first.z * second.y
        );
    }

    public static double Dot(Vector3 first, Vector3 second)
    {
        return first.x * second.x + first.y * second.y + first.z * second.z;
    }
    
    public static double Angle(Vector3 first, Vector3 second)
    {
        return System.Math.Acos(Dot(first, second) / (first.Length * second.Length));
    }

    public static Vector3 operator +(Vector3 left, Vector3 right)
    {
        return new Vector3(
            left.x + right.x,
            left.y + right.y,
            left.z + right.z
        );
    }
    
    public static Vector3 operator +(Vector3 right)
    {
        return new Vector3(
            right.x,
            right.y,
            right.z
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

    public static Vector3 operator -(Vector3 right)
    {
        return new Vector3(
            -right.x,
            -right.y,
            -right.z
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
    
    public static Vector3 operator *(double left, Vector3 right)
    {
        return new Vector3(
            right.x * left,
            right.y * left,
            right.z * left
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