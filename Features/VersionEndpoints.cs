using MoExpenseTracker.Features.V0;

namespace MoExpenseTracker.Features;

static class VersionEndpoints
{
    public static void AddVersionEndpoints(this IEndpointRouteBuilder app)
    {
        // we don't feel good in my mouth saying version zero: 😒
        var versionRoute = app.MapGroup("/");
        versionRoute.AddFeatureEndpointsV0();


        // version 1
        // var versionOneRoute = app.MapGroup("/v1");
        // versionOneRoute.AddFeatureEndpointsV1();
    }
}