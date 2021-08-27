using System;

namespace AppleAuth.Exceptions
{
    /// <summary>
    /// Thrown when one or more parameters is null or invalid.
    /// </summary>
    public class InvalidParametersException : Exception
    {
        internal InvalidParametersException(string message) : base(message) { }
    }
}
