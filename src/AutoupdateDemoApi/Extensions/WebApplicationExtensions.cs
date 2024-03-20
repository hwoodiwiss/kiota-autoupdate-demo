using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace AutoupdateDemoApi.Extensions;

public static class WebApplicationExtensions
{
    public static IEndpointRouteBuilder MapRestFor<TResource>(this IEndpointRouteBuilder routeBuilder)
        where TResource : new()
    {
        var type = typeof(TResource);
        var route = $"/{type.Name.ToLowerInvariant()}s";
        var group = routeBuilder.MapGroup(route)
            .WithOpenApi(o =>
            {
                o.Summary = type.Name;
                o.Description = $"RESTful API for {type.Name}";
                o.Tags = [new OpenApiTag { Name = type.Name }];
                return o;
            });

        group.MapGet("/", () => (TResource[])[new TResource()])
            .Produces<TResource[]>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", (int id) => new TResource())
            .Produces<TResource>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPost("/", (TResource resource) => TypedResults.Json(resource, statusCode: StatusCodes.Status201Created))
            .Produces<TResource>()
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", (int id, TResource resource) => TypedResults.Json(resource))
            .Produces<TResource>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", (int id) => Results.NoContent())
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return routeBuilder;
    }
}
