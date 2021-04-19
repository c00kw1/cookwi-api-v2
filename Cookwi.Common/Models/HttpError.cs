using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cookwi.Common.Models
{
    public class HttpError
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("fields")]
        public List<HttpFieldError> Fields { get; set; }

        public HttpError(string message = "An error has occured", List<HttpFieldError> fieldsErrors = null)
        {
            Message = message;
            Fields = fieldsErrors ?? new List<HttpFieldError>();
        }
    }

    public class HttpFieldError
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        public HttpFieldError(string name, string error)
        {
            Name = name;
            Error = error;
        }
    }
}
