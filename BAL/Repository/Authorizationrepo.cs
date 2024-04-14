using DAL.DataContext;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using BAL.Interface;


namespace BAL.Repository
{
    public class Authorizationrepo
    {

        //-----------------------------------------
        public class CustomAuthorize : Attribute, IAuthorizationFilter
        {


            private readonly string[] _role;
            private readonly ApplicationDbContext _context;
            public CustomAuthorize(string[] role)
            {
                this._role = role;
            }
            public void OnAuthorization(AuthorizationFilterContext context)
          {
                var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();

                var email = context.HttpContext.Session.GetString("Email");
                var roLe = context.HttpContext.Session.GetString("Role");

                if (jwtService == null)
                {
                   context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Patient_login" }));
                   return;
                }
                var request = context.HttpContext.Request;
                var token = request.Cookies["jwt"];
                if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
                {
                  context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Patient_login" }));
                  return;
                }

              

                var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Role");
                //Redirect to Login if not logged in
                if (roleClaim == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Patient_login" }));
                    return;
                }
                //Redirect to Access Denied only if roles mismatch      
                if (_role.Length < 1 || !_role.Contains(roleClaim.Value))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                    return;
                }
           }
            //-----------------------------------------         
            //-----------------------------------------

            //public class CustomAuthorize : Attribute, IAuthorizationFilter
            //{
            //    private readonly string _role;
            //    private readonly ApplicationDbContext _context;
            //    public CustomAuthorize(string role = "")
            //    {
            //        this._role = role;
            //    }
            //    public void OnAuthorization(AuthorizationFilterContext context)
            //    {
            //        var email = context.HttpContext.Session.GetString("Email");
            //        var roLe = context.HttpContext.Session.GetString("Role");

            //        if (email == null)
            //        {
            //            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Login", Action = "Patient_login" }));
            //            return;
            //        }
            //        if (!string.IsNullOrEmpty(_role))
            //        {
            //            if (!(roLe == _role))
            //            {
            //                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "AccessDenied" }));
            //            }
            //        }
            //    }

                //-----------------------------------------         
            }
    }
}
