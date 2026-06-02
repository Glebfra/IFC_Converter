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

        [Property("ASFBR", converter: typeof(AvevaBoolPropertyConverter))] public bool Asfbr = default;
        [Property("BRANCH")] public string Branch = string.Empty;
        [Property("BUIL", converter: typeof(AvevaBoolPropertyConverter))] public bool Buil = default;
        [Property("CSFBR", converter: typeof(AvevaBoolPropertyConverter))] public bool Csfbr = default;
        [Property("DELDSG", converter: typeof(AvevaBoolPropertyConverter))] public bool Deldsg = default;
        [Property("DPGRID")] public int Dpgrid = default;
        [Property("HREL", converter: typeof(AvevaBoolPropertyConverter))] public bool Hrel = default;
        [Property("ISOH", converter: typeof(AvevaBoolPropertyConverter))] public bool Isoh = default;
        [Property("ISPE")] public string Ispe = string.Empty;
        [Property("LEAV")] public int Leav = default;
        [Property("LEXCES")] public double Lexces = default;
        [Property("LOCK", converter: typeof(AvevaBoolPropertyConverter))] public bool Lock = default;
        [Property("LOOS", converter: typeof(AvevaBoolPropertyConverter))] public bool Loos = default;
        [Property("LSFBR", converter: typeof(AvevaBoolPropertyConverter))] public bool Lsfbr = default;
        [Property("LSTU")] public string Lstu = string.Empty;
        [Property("NAME")] public string Name = string.Empty;
        [Property("NWELDS")] public int Nwelds = default;

        [Property("ORI", converter: typeof(AvevaMatrixPropertyConverter))]
        public Matrix<double> Ori = default!;

        [Property("ORIL", converter: typeof(AvevaBoolPropertyConverter))] public bool Oril = default;
        [Property("OWNER")] public string Owner = string.Empty;
        [Property("PIPE")] public string Pipe = string.Empty;

        [Property("POS", converter: typeof(AvevaVectorPropertyConverter))]
        public Vector<double> Pos = default!;

        [Property("POSI", converter: typeof(AvevaBoolPropertyConverter))] public bool Posi = default;
        [Property("PTNO")] public int Ptno = default;
        [Property("PTNT")] public int Ptnt = default;
        [Property("RLOC")] public int Rloc = default;
        [Property("SHOP", converter: typeof(AvevaBoolPropertyConverter))] public bool Shop = default;
        [Property("SITE")] public string Site = string.Empty;
        [Property("SPRE")] public string Spre = string.Empty;
        [Property("TSFBR", converter: typeof(AvevaBoolPropertyConverter))] public bool Tsfbr = default;
        [Property("TSPE")] public string Tspe = string.Empty;
        [Property("TYPE")] public string Type = string.Empty;
        [Property("WORLD")] public string World = string.Empty;
        [Property("ZONE")] public string Zone = string.Empty;
    }
    #pragma warning restore CS0414
}