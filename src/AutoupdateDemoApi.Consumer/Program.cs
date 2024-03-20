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

builder.Services.AddTransient<IRequestAdapter>((sp) =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClientFactory.CreateClient());
});
builder.Services.AddTransient<AutoupdateDemoApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", (AutoupdateDemoApiClient client) =>
{
    return "test";
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
