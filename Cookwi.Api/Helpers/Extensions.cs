using System.Collections.Generic;
using Cookwi.Common.Models;
using FluentValidation.Results;

namespace Cookwi.Api.Helpers
{
    public static class Extensions
    {
        public static HttpError ToHttpError(this ValidationResult validatorResult, string globalErrorMessage)
        {
            var fieldsList = new List<HttpFieldError>();
            foreach (var field in validatorResult.Errors)
            {
                fieldsList.Add(new HttpFieldError(field.PropertyName, field.ErrorMessage));
            }

            var error = new HttpError(globalErrorMessage, fieldsList);

            return error;
        }

        public static double Mb(this int number)
        {
            return number * 1024 * 1024;
        }
    }
}
