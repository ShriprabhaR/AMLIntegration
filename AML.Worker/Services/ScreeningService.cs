using AML.Shared.Models;
using AML.Worker.Configuration;
using AML.Worker.Repositories;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AML.Worker.Services
{
    public class ScreeningService
    {
        private readonly HttpClient _httpClient;
        private readonly AMLSettings _settings;
        private readonly ILogger<ScreeningService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ScreeningService(
            HttpClient httpClient,
            IOptions<AMLSettings> settings,
            ILogger<ScreeningService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task<ScreeningResponse> ScreenCustomerAsync(List<ScreeningRequest> request,string token)
        {
            try
            {
                if (_settings.UseMockAuth)
                {
                    _logger.LogInformation("Using MOCK AML screening response");

                    return new ScreeningResponse
                    {
                        statusCode = 2,
                        message = "Success",
                        results = request.Select(x => new ScreeningResult
                        {
                            customerId = x.customerId,
                            description = "Customer Processed Successfully",
                            message = "Success"
                        }).ToList()
                    };
                }

                var httpRequest = new HttpRequestMessage(HttpMethod.Post,_settings.ScreeningUrl)
                {
                    Content = JsonContent.Create(request)
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(httpRequest);

                var responseText = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "AML screening failed: {Response}",
                        responseText);

                    throw new Exception("AML screening request failed");
                }

                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ScreeningResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AML screening");

                using var scope = _scopeFactory.CreateScope();

                var errorRepository = scope.ServiceProvider
                    .GetRequiredService<ErrorLogRepository>();

                await errorRepository.LogErrorAsync(
                    "AuthService - GetToken",
                    ex.Message,
                    ex.StackTrace);
                throw;
            }
        }
    }
}