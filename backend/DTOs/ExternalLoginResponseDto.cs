using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace backend.DTOs;

public class ExternalLoginResponseDto
{
    [JsonPropertyName("Status_Code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("Message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("Response_Body")]
    public List<ExternalLoginResponseBodyItemDto>? ResponseBody { get; set; }
}

public class ExternalLoginResponseBodyItemDto
{
    [JsonPropertyName("Email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("Doc_Msg")]
    public string DocMsg { get; set; } = string.Empty;

    [JsonPropertyName("User_Locations")]
    public List<UserLocationDto>? UserLocations { get; set; }
}

public class UserLocationDto
{
    [JsonPropertyName("Location_Code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("Location_Name")]
    public string LocationName { get; set; } = string.Empty;
}
