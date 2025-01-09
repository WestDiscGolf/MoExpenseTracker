using MoExpenseTracker.Features;

namespace MoExpenseTracker;

static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServices();

        var app = builder.Build();

        app.UseExceptionHandler(_ => { });
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.AddVersionEndpoints();

        app.Run();
    }

}