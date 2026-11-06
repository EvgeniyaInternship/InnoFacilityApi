using FacilityApi.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLayerServices(builder.Configuration);
builder.Services.AddKeycloakAuthentication(builder.Configuration);
builder.Services.AddApiControllers();
builder.Services.AddSwaggerDocumentation(builder.Configuration);
builder.Services.AddExceptionHandler();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwaggerDocumentation();
app.UseApiMiddlewares();

app.Run();
