using System.Globalization;

using FluentValidation;

namespace MoExpenseTracker.Features.V0.Expense;


static class ExpenseValidationUtil
{
    public static bool IsValidDate(string? date)
    {
        if (string.IsNullOrWhiteSpace(date))
        {
            return true;
        }

        return DateTime.TryParseExact(
            date,
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    public static bool IsValidCategoryId(int? id) => id is null || id > 0;

    public static bool IsValidAmount(decimal? amount) => amount is null || amount >= 0;

    public static bool IsSomeValidString(string? someString)
    {
        return string.IsNullOrWhiteSpace(someString)
               || someString.Length > 5
               || someString.Length <= 255;
    }
}

class ExpenseCreationValidation : AbstractValidator<CreateExpenseDto>
{
    public ExpenseCreationValidation()
    {
        RuleFor(dto => dto.CategoryId)
            .NotNull()
            .GreaterThan(0);

        RuleFor(dto => dto.Amount)
           .NotNull()
           .GreaterThanOrEqualTo(0);

        RuleFor(dto => dto.Description)
            .NotNull()
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(255);

        RuleFor(dto => dto.ExpenseDate)
           .Must(ExpenseValidationUtil.IsValidDate)
           .WithMessage("ExpenseDate must be in the format dd/MM/yyyy when provided.");
    }
}

class ExpenseUpdateValidation : AbstractValidator<UpdateExpenseDto>
{
    public ExpenseUpdateValidation()
    {
        RuleFor(dto => dto.CategoryId)
            .Must(ExpenseValidationUtil.IsValidCategoryId)
            .WithMessage("Category Id must be a positive number when passwed");

        RuleFor(dto => dto.Amount)
          .Must(ExpenseValidationUtil.IsValidAmount)
          .WithMessage("Amount must be a decimal greather than or equal to 0");

        RuleFor(dto => dto.Description)
            .Must(ExpenseValidationUtil.IsSomeValidString)
            .WithMessage("Description must be atleast 5 and at most 255 characters when provided");

        RuleFor(dto => dto.ExpenseDate)
            .Must(ExpenseValidationUtil.IsValidDate)
            .WithMessage("ExpenseDate must be in the format dd/MM/yyyy when provided.");
    }
}