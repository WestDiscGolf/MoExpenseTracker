using MoExpenseTracker.Models;

namespace MoExpenseTracker.Features.V0.Account;

class UserProfileDto(User user)
{
    public string Email { get; set; } = user.Email;
    public int Id { get; set; } = user.Id;
    public string Name { get; set; } = user.Name;
    public DateTime CreatedAt { get; set; } = user.CreatedAt;
    public DateTime UpdatedAt { get; set; } = user.UpdatedAt;
}

class UpdateProfileDto
{
    public required string Email { get; set; }
    public required string Name { get; set; }
}