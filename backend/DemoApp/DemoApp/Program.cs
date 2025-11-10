using DemoApp.Application;
using DemoApp.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register InputProcessor
builder.Services.AddSingleton<IInputProcessor, InputProcessor>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        policy.WithOrigins(
         "http://localhost:5173",   // Vite default port
         "http://localhost:8000"    // Docker nginx proxy
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Content-Type");
    });
});

var app = builder.Build();

// Use CORS - must be before other middleware
app.UseCors("AllowReactClient");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithOpenApi();

app.MapGet("/process/{requestId}", async (
    HttpRequest request, 
    string requestId,  
    string input, 
    IInputProcessor processor,
    ILogger<Program> logger) =>
{
    var response = request.HttpContext.Response;
    response.Headers.Append("Content-type", "text/event-stream");
    response.Headers.Append("Cache-Control", "no-cache");
    response.Headers.Append("Connection", "keep-alive");
    logger.LogInformation("Processing request started for RequestId: {RequestId}, Input: {Input}", requestId, input);
    
    try
	{
        var token = processor.StartProcessingInput(requestId, input);
        var random = new Random();
        var (character, advance) = processor.GetNextProcessedData(requestId);
        logger.LogDebug("Starting character processing loop for RequestId: {RequestId}", requestId);
        while (advance < 100)
        {
            await response.WriteAsync($"data: {character}, {advance}\n\n");
            await response.Body.FlushAsync();
            await Task.Delay(TimeSpan.FromSeconds(random.Next(1,1)), token);
            (character, advance) = processor.GetNextProcessedData(requestId);
        }
        await response.WriteAsync($"data: {character}, {advance}\n\n");
        await response.Body.FlushAsync();
        await response.WriteAsync($"data: done\n\n");
        await response.Body.FlushAsync();

        logger.LogInformation("Processing completed successfully for RequestId: {RequestId}", requestId);
    }
	catch (OperationCanceledException)
	{
        logger.LogWarning("Processing was cancelled for RequestId: {RequestId}", requestId);
        await response.WriteAsync($"data: cancelled\n\n");
        await response.Body.FlushAsync();
    }
    catch (Exception e)
	{
        logger.LogError(e, "Error occurred while processing RequestId: {RequestId}", requestId);
        await response.WriteAsync($"data: error - {e.Message}\n\n");
        await response.Body.FlushAsync();
    }
    finally
    {
       logger.LogDebug("Cleaning up processing for RequestId: {RequestId}", requestId);
       processor.CancelProcessing(requestId);
    }

});

app.MapPost("/process/{requestId}/cancel", (
    string requestId,
    IInputProcessor processor,
    ILogger<Program> logger) =>
    {

        logger.LogInformation("Cancelling process for RequestId: {RequestId}", requestId);
        return processor.CancelProcessing(requestId)
        ? Results.Ok("cancelled")
        : Results.NotFound(requestId);
    });


app.Run();
