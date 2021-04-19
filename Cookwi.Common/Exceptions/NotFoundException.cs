using System.Net;
using Cookwi.Common.Models;

namespace Cookwi.Common.Exceptions
{
    public class NotFoundException : HttpException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public NotFoundException(string message) : this(new HttpError(message))
        {
        }

        public NotFoundException(HttpError error) : base(error)
        {
        }
    }
}
