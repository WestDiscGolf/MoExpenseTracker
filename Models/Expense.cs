namespace MoExpenseTracker.Models;

class Expense
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public string? Description { get; set; }
    public DateTime? ExpenseDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}