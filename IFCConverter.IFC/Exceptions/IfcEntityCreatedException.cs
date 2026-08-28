using System;

namespace IFCConverter.IFC.Exceptions
{
    public class IfcEntityCreatedException : Exception
    {
        public IfcEntityCreatedException()
        {
        }

        public IfcEntityCreatedException(string message) : base(message)
        {
        }
    }
}