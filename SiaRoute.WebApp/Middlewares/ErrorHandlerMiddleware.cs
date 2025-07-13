using System.Net;
using System.Text.Json;
using Application.Exceptions;
using Application.Wrappers;

namespace SiaRoute.WebApp.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                List<string> message = new List<string>();
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };
                context.Response.Clear();

                switch (error)
                {
                    case ApiException e:
                        message.Add(e.Message);
                        responseModel.Errors = message;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case ValidationException e: 
                        message.Add(e.Message);
                        responseModel.Errors = message;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = e.Errors;
                        break;
                    case KeyNotFoundException e: 
                        message.Add(e.Message);
                        responseModel.Errors = message;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case UnauthorizedAccessException e: 
                        message.Add(e.Message);
                        responseModel.Errors = message;
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case NotImplementedException e: 
                        message.Add(e.Message);
                        responseModel.Errors = message;
                        response.StatusCode = (int)HttpStatusCode.NotImplemented;
                        break;
                    default: 
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
                };
                var result = JsonSerializer.Serialize(responseModel,options);

                await response.WriteAsync(result);
            }
        }
    }
}
