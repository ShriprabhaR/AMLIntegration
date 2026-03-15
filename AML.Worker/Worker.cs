using AML.Shared.Models;
using AML.Shared.Models.Profiler;
using AML.Worker.Repositories;
using AML.Worker.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AML.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AuthService _authService;
        private readonly ScreeningService _screeningService;
        private readonly ILogger<Worker> _logger;
        private readonly IMapper _mapper;

        private const int BatchSize = 100;

        public Worker(
            IServiceScopeFactory scopeFactory,
            AuthService authService,
            ScreeningService screeningService,
            ILogger<Worker> logger,
            IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _authService = authService;
            _screeningService = screeningService;
            _logger = logger;
            _mapper = mapper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AML Worker Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider.GetRequiredService<CustomerRepository>();
                var errorRepo = scope.ServiceProvider.GetRequiredService<ErrorLogRepository>();

                try
                {
                    var token = await _authService.GetToken();

                    var customers = await repository.GetCustomersAsync();

                    if (!customers.Any())
                    {
                        _logger.LogInformation("No customers found");
                        await Task.Delay(5000, stoppingToken);
                        continue;
                    }

                    _logger.LogInformation("Total Customers Fetched: {Count}", customers.Count);

                    var batches = customers
                        .Select((customer, index) => new { customer, index })
                        .GroupBy(x => x.index / BatchSize)
                        .Select(x => x.Select(v => v.customer).ToList())
                        .ToList();

                    _logger.LogInformation("Total Batches Created: {BatchCount}", batches.Count);

                    int batchNumber = 1;

                    foreach (var batch in batches)
                    {
                        if (stoppingToken.IsCancellationRequested)
                            break;

                        _logger.LogInformation("Processing Batch {BatchNumber}", batchNumber);

                        try
                        {
                            // Use AutoMapper instead of manual mapping
                            var requests = _mapper.Map<List<ScreeningRequest>>(batch);

                            var response = await _screeningService
                                .ScreenCustomerAsync(requests, token);

                            var resultList = response.results.Select(r => new
                            {
                                r.customerId,
                                r.description
                            });

                            _logger.LogInformation(
                                "Batch {BatchNumber} processed successfully. Results: {@Results}",
                                batchNumber,
                                resultList);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Batch {BatchNumber} failed", batchNumber);

                            await errorRepo.LogErrorAsync(
                                "Batch Screening",
                                ex.Message,
                                ex.StackTrace);
                        }

                        batchNumber++;

                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker Error");

                    await errorRepo.LogErrorAsync(
                        "Worker Execution",
                        ex.Message,
                        ex.StackTrace);
                }
            }
        }
    }
}