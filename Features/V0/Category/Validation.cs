using FluentValidation;

namespace MoExpenseTracker.Features.V0.Category;


class CategoryCreationValidation : AbstractValidator<CreateCategoryDto>
{
    public CategoryCreationValidation()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(dto => dto.Description)
            .NotEmpty()
            .NotNull()
            .MinimumLength(10)
            .MaximumLength(255);
    }
}

class CategoryUpdateValidation : AbstractValidator<UpdateCategoryDto>
{
    public CategoryUpdateValidation()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .NotNull()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(dto => dto.Description)
            .NotEmpty()
            .NotNull()
            .MinimumLength(10)
            .MaximumLength(255);
    }
}