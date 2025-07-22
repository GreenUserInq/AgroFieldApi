using AgroFieldApi.Services;
using AgroFieldApi.Settings;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ����������� ������������ KmlPathSettings �� appsettings.json
builder.Services.Configure<KmlPathSettings>(
    builder.Configuration.GetSection("KmlPaths"));

builder.Services.AddSingleton<FieldService>();

builder.Services.AddControllers();

// ���������� � ��������� Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AgroField API",
        Version = "v1",
        Description = "API ��� ������ � KML-������� ������� �����"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
