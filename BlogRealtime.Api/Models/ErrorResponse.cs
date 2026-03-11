namespace BlogRealtime.Api.Models;

public class ErrorResponse
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
}
