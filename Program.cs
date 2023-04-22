using GremlinApi;
using GremlinAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IGraphService, GraphService>(); // Register the IGraphService interface with its implementation, GraphService.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
// Create the Cosmos DB database and graph if they don't exist.
using (var scope = app.Services.CreateScope()) // Create a new service scope to access the IGraphService implementation.
{
    var services = scope.ServiceProvider; // Get the ServiceProvider instance from the scope.
    var loggerFactory = services.GetRequiredService<ILoggerFactory>(); // Retrieve the ILoggerFactory service from the ServiceProvider.
    try
    {
        var graphService = services.GetRequiredService<IGraphService>(); // Retrieve the IGraphService service from the ServiceProvider.
        await graphService.CreateDatabaseAndGraphIfNotExistsAsync(); // Call the CreateDatabaseAndGraphIfNotExistsAsync method to create the database and graph if they don't exist.
    }
    catch (Exception ex) // Catch any exceptions that might occur during database and graph creation.
    {
        var logger = loggerFactory.CreateLogger<Program>(); // Create a logger instance to log the exception.
        logger.LogError(ex, "An error occurred while creating the database and graph."); // Log the exception with a message.
    }
}

app.Run();