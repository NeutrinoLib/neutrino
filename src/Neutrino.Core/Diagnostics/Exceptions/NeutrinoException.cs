using System;
using System.Net;

namespace Neutrino.Core.Diagnostics.Exceptions
{
    public class NeutrinoException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;

        public NeutrinoException()
        {
        }

        public NeutrinoException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public NeutrinoException(string message) : base(message)
        {
        }

        public NeutrinoException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}