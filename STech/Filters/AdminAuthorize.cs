using Microsoft.AspNetCore.Mvc.Filters;

namespace STech.Filters
{
    public class AdminAuthorize : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(!context.HttpContext.User.IsInRole("admin"))
            {
                context.HttpContext.Response.Redirect("/admin/login");
            }
               
        }
    }
}
