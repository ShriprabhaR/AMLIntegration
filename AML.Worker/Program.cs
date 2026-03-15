using AML.Worker;
using AML.Shared.Infrastructure;
using AML.Worker.Services;
using AML.Worker.Configuration;
using AML.Worker.Repositories;
using Polly;
using Polly.Retry;
using Microsoft.Data.SqlClient;
using System.Data;
using AML.Shared.Models.Profiler;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSingleton<SqlConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new SqlConnectionFactory(
        configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.Configure<AMLSettings>(builder.Configuration.GetSection("AMLSettings"));
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<ScreeningService>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<ErrorLogRepository>();
builder.Services.AddAutoMapper(typeof(AMLMappingProfile));

var pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 })
    .Build();




var host = builder.Build();
host.Run();
