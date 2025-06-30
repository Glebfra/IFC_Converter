using System;

namespace IFC.Exceptions
{
    public class IfcConvertException : Exception
    {
        public string Exception;

        public IfcConvertException(string exception) : base(exception)
        {
            Exception = exception;
        }
    }
}