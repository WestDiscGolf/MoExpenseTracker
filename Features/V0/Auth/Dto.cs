namespace MoExpenseTracker.Features.Auth
{
    class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    class SignupDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    class AuthResponseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string AccessToken { get; set; }
    }
}