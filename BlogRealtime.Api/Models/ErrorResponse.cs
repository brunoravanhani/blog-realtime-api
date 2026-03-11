namespace BlogRealtime.Api.Models;

public class ErrorResponse
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
    public List<ValidationError> Errors { get; set; }
}

public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}