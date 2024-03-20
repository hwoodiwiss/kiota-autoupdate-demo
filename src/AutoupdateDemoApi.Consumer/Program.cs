using AutoupdateDemoApi.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpClientDefaults(client =>
{
    client.ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://localhost:50001");
        c.DefaultRequestHeaders.Add("User-Agent", "AutoupdateDemoApi.Consumer");
    });
});

builder.Services.AddScoped<IRequestAdapter>((sp) =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClientFactory.CreateClient());
});
builder.Services.AddScoped<AutoupdateDemoApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/them-apples", async (AutoupdateDemoApiClient client) =>
{
    var apples = await client.Apples.GetAsync();
    return apples?.Select(apple => new AppleDto(apple.Id, apple.Name)).ToArray() ?? [];
})
.WithName("Apples")
.WithOpenApi();

app.Run();

public sealed record AppleDto(int? Id, string? Name);
