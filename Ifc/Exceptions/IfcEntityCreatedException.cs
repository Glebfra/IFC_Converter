using System;

namespace Ifc.Exceptions
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