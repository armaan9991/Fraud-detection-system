using Hangfire.Dashboard;

namespace FraudDetection.API.Infrastructure;
public class HangfireAuthorizationFilter : IDashboardAsyncAuthorizationFilter, IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var HttpContext = context.GetHttpContext();
        return HttpContext.User.Identity?.IsAuthenticated == true && HttpContext.User.IsInRole("Admin");
    }
    public Task<bool> AuthorizeAsync(DashboardContext context)
    {
        return Task.FromResult(Authorize(context));
    }
}