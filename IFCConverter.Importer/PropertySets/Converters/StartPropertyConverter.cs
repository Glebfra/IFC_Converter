using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Importer.PropertySets.Converters
{
    internal delegate bool TryParseHandler<T>(string value, NumberStyles numberStyles, IFormatProvider formatProvider, out T result);

    internal sealed class StartPropertyConverter<TOutput> : AbstractPropertyConverter<IfcValue, TOutput>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Regex NumberRegex = new Regex(@"[-+]?(?:\d+(?:[.,]\d*)?|[.,]\d+)(?:[eE][-+]?\d+)?", RegexOptions.Compiled);

        public override TOutput ReadTyped(IfcValue source)
        {
            string value = ReadNumber(source).Replace(',', '.');

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TOutput));
            try
            {
                object result = converter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
                if (result is TOutput typedResult)
                    return typedResult;
            }
            catch (Exception exception)
                when (exception is FormatException || exception is NotSupportedException || exception is OverflowException)
            {
                throw new FormatException($"Cannot parse '{value}' as {typeof(TOutput).Name}.", exception);
            }

            throw new FormatException($"Cannot parse '{value}' as {typeof(TOutput).Name}.");
        }

        private static string ReadNumber(IfcValue source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            string text = source.Value?.ToString();
            if (string.IsNullOrWhiteSpace(text))
                throw new FormatException("The START property contains an empty value");

            Match match = NumberRegex.Match(text);
            if (!match.Success)
                throw new FormatException($"No numeric value found in text '{text}'.");

            return match.Value;
        }
    }
}