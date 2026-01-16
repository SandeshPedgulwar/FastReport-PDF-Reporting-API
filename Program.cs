using Transaction_Reports_App.ApplicationLayer.Services.Abstract;
using Transaction_Reports_App.ApplicationLayer.Services.Concrete;
using DotNetEnv;
using Serilog;


Env.Load();
string logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Debug";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(Enum.Parse<Serilog.Events.LogEventLevel>(logLevel))
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddScoped<ITransactionService, TransactionService>();
}

    // Add services to the container.

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

app.Run();
