using Clywell.Core.Logging.Extensions;
using StudentScoreApi.Services;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<StudentService>();

// Clywell Logging Configuration
// NOTE: Method name in source is AddLogging, differs from README's AddClywellLogging
builder.AddLogging(config => 
{
    config
        .WithMinimumLevel(LogEventLevel.Debug)
        .WithConsoleSink()
        .WithClywellDefaults();
});

var app = builder.Build();

// Clywell Middleware
// NOTE: Method names in source are UseRequestTracking/UseRequestLogging
app.UseRequestTracking();
app.UseRequestLogging();

app.UseAuthorization();
app.MapControllers();

app.Run();
