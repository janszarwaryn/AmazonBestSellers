namespace AmazonBestSellers.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public required string EntityId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}
