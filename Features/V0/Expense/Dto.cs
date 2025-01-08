namespace MoExpenseTracker.Features.V0.Expense;

public class CreateExpenseDto
{
    public required int CategoryId { get; set; }
    public required decimal Amount { get; set; }
    public required string Description { get; set; }

    public string? ExpenseDate { get; set; }
}

public class UpdateExpenseDto
{
    public int? CategoryId { get; set; }
    public decimal? Amount { get; set; }
    public string? Description { get; set; }
    public string? ExpenseDate { get; set; }
}