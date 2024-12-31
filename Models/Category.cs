namespace MoExpenseTracker.Models;

class Category
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}