using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.PropertySets.Converters;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.PropertySets.Aveva
{
    #pragma warning disable CS0414
    [PropertySet("AVEVA_Pset", PropertyMatchMode.StartsWith)]
    internal class AvevaPset : AbstractPropertySet
    {
        [Property("AEXCES")] public string Aexces = string.Empty;
        [Property("ARRI")] public string Arri = string.Empty;

        [Property("ASFBR")] public bool Asfbr = default;
        [Property("BRANCH")] public string Branch = string.Empty;
        [Property("BUIL")] public bool Buil = default;
        [Property("CSFBR")] public bool Csfbr = default;
        [Property("DELDSG")] public bool Deldsg = default;
        [Property("DPGRID")] public int Dpgrid = default;
        [Property("HREL")] public bool Hrel = default;
        [Property("ISOH")] public bool Isoh = default;
        [Property("ISPE")] public string Ispe = string.Empty;
        [Property("LEAV")] public int Leav = default;
        [Property("LEXCES")] public double Lexces = default;
        [Property("LOCK")] public bool Lock = default;
        [Property("LOOS")] public bool Loos = default;
        [Property("LSFBR")] public bool Lsfbr = default;
        [Property("LSTU")] public string Lstu = string.Empty;
        [Property("NAME")] public string Name = string.Empty;
        [Property("NWELDS")] public int Nwelds = default;

        [Property("ORI", converter: typeof(AvevaMatrixPropertyConverter))]
        public Matrix<double> Ori = default!;

        [Property("ORIL")] public bool Oril = default;
        [Property("OWNER")] public string Owner = string.Empty;
        [Property("PIPE")] public string Pipe = string.Empty;

        [Property("POS", converter: typeof(AvevaVectorPropertyConverter))]
        public Vector<double> Pos = default!;

        [Property("POSI")] public bool Posi = default;
        [Property("PTNO")] public int Ptno = default;
        [Property("PTNT")] public int Ptnt = default;
        [Property("RLOC")] public int Rloc = default;
        [Property("SHOP")] public bool Shop = default;
        [Property("SITE")] public string Site = string.Empty;
        [Property("SPRE")] public string Spre = string.Empty;
        [Property("TSFBR")] public bool Tsfbr = default;
        [Property("TSPE")] public string Tspe = string.Empty;
        [Property("TYPE")] public string Type = string.Empty;
        [Property("WORLD")] public string World = string.Empty;
        [Property("ZONE")] public string Zone = string.Empty;
    }
    #pragma warning restore CS0414
}