using System.Text.Json.Serialization;

namespace backend.DTOs;

public class ExternalLoginResponseDto
{
    [JsonPropertyName("Status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("Message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("User_Locations")]
    public List<UserLocationDto> UserLocations { get; set; } = [];
}

public class UserLocationDto
{
    [JsonPropertyName("Location_Code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("Location_Name")]
    public string LocationName { get; set; } = string.Empty;
}
