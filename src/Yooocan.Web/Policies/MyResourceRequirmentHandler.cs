using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Yooocan.Web.Policies
{
    public class MyResourceRequirmentHandler : AuthorizationHandler<MyResourceRequirment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyResourceRequirment requirement)
        {
            var mvcContext = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
            if (mvcContext == null)
                return Task.FromResult(0);

            var resourceId = mvcContext.RouteData.Values["id"];
            if (context.User.HasClaim(requirement.ResourceName, resourceId.ToString()) || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}