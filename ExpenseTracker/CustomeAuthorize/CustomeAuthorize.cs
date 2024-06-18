//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.AspNetCore.Mvc;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//namespace ExpenseTracker.CustomeAuthorize
//{
//    public class CustomeAuthorize
//    {
//    }
//}




using ExpenseTrackerService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExpenseTracker.AuthMIddleware
{
    public class CustomeAuthorize : Attribute, IAuthorizationFilter
    {

       

        //public CustomeAuthorize()
        //{
        //}

        //public CustomeAuthorize(IExpenseService service)
        //{
        //    _service = service;
        //}
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var request = context.HttpContext.Request;
            var token = request.Cookies["myToken"];
            var service = context.HttpContext.RequestServices.GetService<IExpenseService>();

            if (token == null || (service?.ValidateToken(token)==0))
            {
                context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary(new
                {
                    Controller = "Login",
                    Action = "Login",
                }));
                return;
            }

        }
           
    }

}
