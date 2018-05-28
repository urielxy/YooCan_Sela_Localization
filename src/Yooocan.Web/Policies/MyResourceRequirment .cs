using Microsoft.AspNetCore.Authorization;

namespace Yooocan.Web.Policies
{
    public class MyResourceRequirment : IAuthorizationRequirement
    {
        public string ResourceName { get; set; }

        public MyResourceRequirment(string resourceName)
        {
            ResourceName = resourceName;
        }
    }
}