namespace ConcurExpense;

public class ConcurOptions
{
    public const string SectionName = "ConcurExpense";

    public string BaseUrl { get; set; } = "https://us.api.concursolutions.com";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string? ProxyUrl { get; set; }
    public string? ProxyUsername { get; set; }
    public string? ProxyPassword { get; set; }
}
