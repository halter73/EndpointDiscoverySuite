using Microsoft.AspNetCore.Routing.Patterns;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IStartupFilter, EndpointAddingStartupFilter>();
var app = builder.Build();

app.MapGet("/0", () => "Hello World!");
app.MapGet("/1", context => context.Response.WriteAsync("Hello World!"));
app.Map("/2", () => "Hello World!");
app.Map(RoutePatternFactory.Parse("/3"), () => "Hello World!");
app.MapMethods("/4", new[] { "GET" }, () => "Hello World!");
app.MapPost("/5", () => "Hello World!");
app.MapPut("/6", () => "Hello World!");
app.MapDelete("/7", () => "Hello World!");

var pattern8 = "/8";
var helloRouteHandler = () => "Hello World!";
app.Map(pattern8, helloRouteHandler);

var emptyGroup = app.MapGroup("");
var nestedGroup = emptyGroup.MapGroup("");
var prefixedGroup = app.MapGroup("prefix");
var nestedPrefixGroup = prefixedGroup.MapGroup("nested");

emptyGroup.MapGet("/9", () => "Hello World!");
emptyGroup.MapGet("/10", context => context.Response.WriteAsync("Hello World!"));
nestedGroup.MapGet("/11", () => "Hello World!");
nestedPrefixGroup.MapGet("/", () => "Hello World!"); // This route here is "/prefix/nested"

#pragma warning disable ASP0014 // Suggest using top level route registrations instead of UseEndpoints
app.UseRouting();
app.UseEndpoints(routeBuilder =>
{
    routeBuilder.MapGet("/12", () => "Hello World!");
});
#pragma warning restore ASP0014 // Suggest using top level route registrations instead of UseEndpoints

AddStaticEndpoints(app);

// I don't expect we'll get anything below this line working at design time.
AddParameterizedEndpoints(app, 15, helloRouteHandler);

AddParameterizedEndpoints(emptyGroup, 17, helloRouteHandler);
AddParameterizedEndpoints(nestedGroup, 19, helloRouteHandler);

app.Run();

static void AddStaticEndpoints(WebApplication app)
{
    app.MapGet("/13", () => "Hello World!");
    app.MapGet("/14", context => context.Response.WriteAsync("Hello World!"));
}

static void AddParameterizedEndpoints(IEndpointRouteBuilder routeBuilder, int startingIndex, Delegate handler)
{
    for (int i = 0; i < 2; i++)
    {
        routeBuilder.MapGet($"/{startingIndex + i}", handler);
    }
}

class EndpointAddingStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            next(app);
            app.UseEndpoints(routeBuilder =>
            {
                routeBuilder.MapGet("/21", () => "Hello World!");
            });
        };
    }
}
