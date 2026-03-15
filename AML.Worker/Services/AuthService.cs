using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AML.Shared.Models;
using AML.Worker.Configuration;
using AML.Worker.Repositories;
using Microsoft.Extensions.Options;

namespace AML.Worker.Services
{
    public class AuthService
    {
        private readonly HttpClient _client;
        private readonly AMLSettings _settings;
        private readonly ILogger<AuthService> _logger;
        private string? _cachedToken;
        private DateTime _tokenExpiry;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuthService(HttpClient client, IOptions<AMLSettings> settings, ILogger<AuthService> logger, IServiceScopeFactory scopeFactory)
        {
            _client = client;
            _settings = settings.Value;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        public async Task<string> GetToken()
        {

            if (!string.IsNullOrEmpty(_cachedToken) &&
                DateTime.UtcNow < _tokenExpiry)
            {
                _logger.LogInformation("Using cached AML token");
                return _cachedToken;
            }

            if (_settings.UseMockAuth)
            {
                _logger.LogInformation("Using MOCK AML token");
                _cachedToken = "mock-token-123456789";
                _tokenExpiry = DateTime.UtcNow.AddMinutes(1);
                return _cachedToken;
            }
            try
            {
                //throw new Exception("TEST TOKEN FAILURE");

                var requestBody = new
                {
                    empID = _settings.EmpID,
                    pswd = _settings.Password
                };
                var request = new HttpRequestMessage(HttpMethod.Post, _settings.AuthUrl)
                {
                    Content = JsonContent.Create(requestBody)
                };
                request.Headers.Add("X-Tenant-Name", _settings.TenantName);
                request.Headers.Add("CSRF-TOKEN", _settings.CsrfToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                _logger.LogInformation("Calling AML authentication API");
                _logger.LogInformation("Requesting new AML token");

                var response = await _client.SendAsync(request);
                var responseText = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Token request failed: {Response}", responseText);
                    throw new Exception("AML Token generation failed");
                }

                var tokenResponse =
                    JsonSerializer.Deserialize<TokenResponse>(
                        responseText,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Token))
                    throw new Exception("Invalid AML token response");

                _cachedToken = tokenResponse.Token;
                _tokenExpiry = DateTime.UtcNow.AddMinutes(1);
                return _cachedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AML token");

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