using System;
using System.Net;
using Cookwi.Common.Models;

namespace Cookwi.Common.Exceptions
{
    public abstract class HttpException : Exception
    {
        public abstract HttpStatusCode StatusCode { get; }
        public HttpError HttpError { get; }

        public HttpException(HttpError error) : base(error?.Message)
        {
            HttpError = error;
        }
    }
}
