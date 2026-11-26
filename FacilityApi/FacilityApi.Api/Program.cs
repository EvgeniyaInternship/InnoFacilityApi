using FacilityApi.Api.Extensions;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddLayerServices(builder.Configuration);
builder.Services.AddKeycloakAuthentication(builder.Configuration);
builder.Services.AddApiControllers();
builder.Services.AddSwaggerDocumentation(builder.Configuration);
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.CombineLogs = true;
});
builder.Services.AddExceptionHandler();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseExceptionHandler();
app.UseSwaggerDocumentation();
app.UseApiMiddlewares();

Log.Information("Facility API started.");
app.Run();
