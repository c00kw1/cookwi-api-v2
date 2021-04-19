using System.Net;
using Cookwi.Common.Models;

namespace Cookwi.Common.Exceptions
{
    public class BadRequestException : HttpException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public BadRequestException(string message) : this(new HttpError(message))
        {
        }

        public BadRequestException(HttpError error) : base(error)
        {
        }
    }
}
