using AutoMapper;
using ToDo.Api.Mapping;
using ToDo.Api.Middlewares;
using ToDo.Api.Repositories;
using ToDo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers to the service collection
builder.Services.AddControllers();

// Dependency Injection – register app services here
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddSingleton<ITaskItemRepository, TaskItemRepository>();

// AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<TaskItemProfile>();
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Angular
const string FrontendClient = "FrontendClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendClient, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Global exception handling
app.UseExceptionHandlingMiddleware();

// Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
    });
}

// Common pipeline pieces
app.UseHttpsRedirection();

app.UseCors(FrontendClient);

app.UseAuthorization();

// Map controller routes
app.MapControllers();

app.Run();
