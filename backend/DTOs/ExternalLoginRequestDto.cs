using System.Text.Json.Serialization;

namespace backend.DTOs;

public class ExternalLoginRequestDto
{
    [JsonPropertyName("API_Action")]
    public string ApiAction { get; set; } = string.Empty;

    [JsonPropertyName("Device_Id")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("Sync_Time")]
    public string SyncTime { get; set; } = string.Empty;

    [JsonPropertyName("Company_Code")]
    public string CompanyCode { get; set; } = string.Empty;

    [JsonPropertyName("API_Body")]
    public ExternalLoginBodyDto ApiBody { get; set; } = new();
}

public class ExternalLoginBodyDto
{
    [JsonPropertyName("Username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("Pw")]
    public string Password { get; set; } = string.Empty;
}
