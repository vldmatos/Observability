﻿using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;

namespace Library.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Log.Error(exception, "Exception");

        var code = HttpStatusCode.InternalServerError;

        var result = System.Text.Json.JsonSerializer.Serialize(new { error = exception?.Message });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}