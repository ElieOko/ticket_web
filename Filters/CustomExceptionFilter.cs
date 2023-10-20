using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SCustomers.Helpers;
using SCustomers.Models;
using System;
using System.Net;

namespace SCustomers.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilter:ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is Exceptions.UnauthorizedAccessException accessException)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var apiError = new ApiError(accessException.Message,
                    accessException.ErrorCode);
                context.Result = new JsonResult(apiError);

                return;
            }

            if(context.Exception is Exceptions.NotFoundException foundException)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                var apiError = new ApiError(foundException.Message,
                    foundException.ErrorCode);
                context.Result = new JsonResult(apiError);

                return;
            }

            if (context.Exception is Exceptions.BadOperationException badException)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var apiError = new ApiError(badException.Message,
                    badException.ErrorCode);
                context.Result = new JsonResult(apiError);

                return;
            }

            //Unhandled exception
            //Log it first before return an internal server error
            LogManager.LogException(context.Exception);

            var code = HttpStatusCode.InternalServerError;
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)code;
            context.Result = new JsonResult(
                new { Message = "There is a problem with the server" }
            );

            base.OnException(context);
        }
    }
}
