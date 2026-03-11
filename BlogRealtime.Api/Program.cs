using BlogRealtime.Api.ExtensionMethods;
using BlogRealtime.Application.ExtensionMethods;
using BlogRealtime.Domain.ExtensionMethods;
using BlogRealtime.Domain.Settings;
using BlogRealtime.Infra.ExtensionMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddDomainServices();
builder.Services.AddApplicationServices();
builder.Services.AddApiServices();

builder.Services.AddInfraServices(jwtSettings);

builder.Services.SetupAuthentication(jwtSettings);

builder.Services.SetupSwagger();

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

app.SetupDBSeed();

app.Run();
