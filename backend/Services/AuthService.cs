using System.Text;
using System.Text.Json;
using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly EnhanzerApiOptions _apiOptions;
    private readonly AppDbContext _db;
    private readonly ILogger<AuthService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthService(
        HttpClient httpClient,
        IOptions<EnhanzerApiOptions> apiOptions,
        AppDbContext db,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _apiOptions = apiOptions.Value;
        _db = db;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
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

        var json = JsonSerializer.Serialize(externalRequest);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await _httpClient.PostAsync(_apiOptions.BaseUrl, content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to reach external login API");
            throw new ApplicationException("Unable to reach the authentication service. Please try again later.", ex);
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("External login API returned HTTP {StatusCode}", (int)httpResponse.StatusCode);
            throw new ApplicationException($"External authentication service returned HTTP {(int)httpResponse.StatusCode}.");
        }

        var responseBody = await httpResponse.Content.ReadAsStringAsync();

        ExternalLoginResponseDto? loginResponse;
        try
        {
            loginResponse = JsonSerializer.Deserialize<ExternalLoginResponseDto>(responseBody, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse external login response");
            throw new ApplicationException("Unexpected response from authentication service.", ex);
        }

        if (loginResponse == null || loginResponse.Status != "Success")
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = loginResponse?.Message ?? "Login failed."
            };
        }

        await SaveLocationsAsync(loginResponse.UserLocations);

        return new LoginResponseDto
        {
            Success = true,
            Message = "Login successful."
        };
    }

    private async Task SaveLocationsAsync(List<UserLocationDto> userLocations)
    {
        if (userLocations == null || userLocations.Count == 0)
            return;

        foreach (var location in userLocations)
        {
            if (string.IsNullOrWhiteSpace(location.LocationCode))
                continue;

            var existing = await _db.LocationDetails
                .FirstOrDefaultAsync(l => l.LocationCode == location.LocationCode);

            if (existing == null)
            {
                _db.LocationDetails.Add(new LocationDetail
                {
                    LocationCode = location.LocationCode,
                    LocationName = location.LocationName
                });
            }
            else if (existing.LocationName != location.LocationName)
            {
                existing.LocationName = location.LocationName;
            }
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save location details to the database");
            throw new ApplicationException("Login succeeded but location data could not be saved.", ex);
        }
    }
}
