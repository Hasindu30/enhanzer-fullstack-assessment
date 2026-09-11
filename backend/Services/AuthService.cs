using System.Text;
using System.Text.Json;
using backend.DTOs;
using backend.Models;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly EnhanzerApiOptions _apiOptions;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        HttpClient httpClient,
        IOptions<EnhanzerApiOptions> apiOptions,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _apiOptions = apiOptions.Value;
        _logger = logger;
    }

    public async Task<string> LoginAsync(LoginRequestDto request)
    {
        var externalRequest = new ExternalLoginRequestDto
        {
            ApiAction = _apiOptions.ApiAction,
            DeviceId = _apiOptions.DeviceId,
            SyncTime = string.Empty,
            CompanyCode = request.Email,
            ApiBody = new ExternalLoginBodyDto
            {
                Username = request.Email,
                Password = request.Password
            }
        };

        var jsonContent = JsonSerializer.Serialize(externalRequest);
        using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(_apiOptions.BaseUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("External API returned non-success HTTP status code {StatusCode}", (int)response.StatusCode);
                throw new ApplicationException($"External authentication service returned HTTP {(int)response.StatusCode}.");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to communicate with external login API at {Url}", _apiOptions.BaseUrl);
            throw new ApplicationException("Unable to reach external authentication service. Please try again later.", ex);
        }
    }
}
