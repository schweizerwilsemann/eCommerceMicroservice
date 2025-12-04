using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway (RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            // extract the specific header from the request

            var signedHeader = context.Request.Headers["Api-Gateway"];

            // null means, the request is not coming frm the api gateway
            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
