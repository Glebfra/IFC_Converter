using IFCConverter.Attributes;
using IFCConverter.PropertySets.Converters;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.PropertySets.Aveva
{
    #pragma warning disable CS0414
    [PropertySet(name: "AVEVA_Pset", PropertyMatchMode.StartsWith)]
    internal class AvevaPset : AbstractPropertySet
    {
        [Property(name: "AEXCES")] public string Aexces = string.Empty;
        [Property(name: "ARRI")] public string Arri = string.Empty;

        [Property(name: "ASFBR")] public bool Asfbr = default;
        [Property(name: "BRANCH")] public string Branch = string.Empty;
        [Property(name: "BUIL")] public bool Buil = default;
        [Property(name: "CSFBR")] public bool Csfbr = default;
        [Property(name: "DELDSG")] public bool Deldsg = default;
        [Property(name: "DPGRID")] public int Dpgrid = default;
        [Property(name: "HREL")] public bool Hrel = default;
        [Property(name: "ISOH")] public bool Isoh = default;
        [Property(name: "ISPE")] public string Ispe = string.Empty;
        [Property(name: "LEAV")] public int Leav = default;
        [Property(name: "LEXCES")] public double Lexces = default;
        [Property(name: "LOCK")] public bool Lock = default;
        [Property(name: "LOOS")] public bool Loos = default;
        [Property(name: "LSFBR")] public bool Lsfbr = default;
        [Property(name: "LSTU")] public string Lstu = string.Empty;
        [Property(name: "NAME")] public string Name = string.Empty;
        [Property(name: "NWELDS")] public int Nwelds = default;
        [Property(name: "ORI", converter: typeof(AvevaMatrixPropertyConverter))] public Matrix<double> Ori = default!;
        [Property(name: "ORIL")] public bool Oril = default;
        [Property(name: "OWNER")] public string Owner = string.Empty;
        [Property(name: "PIPE")] public string Pipe = string.Empty;
        [Property(name: "POS", converter: typeof(AvevaVectorPropertyConverter))] public Vector<double> Pos = default!;
        [Property(name: "POSI")] public bool Posi = default;
        [Property(name: "PTNO")] public int Ptno = default;
        [Property(name: "PTNT")] public int Ptnt = default;
        [Property(name: "RLOC")] public int Rloc = default;
        [Property(name: "SHOP")] public bool Shop = default;
        [Property(name: "SITE")] public string Site = string.Empty;
        [Property(name: "SPRE")] public string Spre = string.Empty;
        [Property(name: "TSFBR")] public bool Tsfbr = default;
        [Property(name: "TSPE")] public string Tspe = string.Empty;
        [Property(name: "TYPE")] public string Type = string.Empty;
        [Property(name: "WORLD")] public string World = string.Empty;
        [Property(name: "ZONE")] public string Zone = string.Empty;
    }
    #pragma warning restore CS0414
}