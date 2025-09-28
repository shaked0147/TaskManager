namespace TaskManagerApi.Models;

public class JwtSettings
{
    public string Secret { get; set; }
    public int ExpiryHours { get; set; }
}
