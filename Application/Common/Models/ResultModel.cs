namespace Application.Common.Models;

public class ResultModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int ErrorCode { get; set; } = 200;
}