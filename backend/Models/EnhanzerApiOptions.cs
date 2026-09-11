namespace backend.Models;

public class EnhanzerApiOptions
{
    public const string SectionName = "EnhanzerApi";

    public string BaseUrl { get; set; } = string.Empty;
    public string ApiAction { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}
