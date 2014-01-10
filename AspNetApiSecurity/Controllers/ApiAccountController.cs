using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using AspNetApiSecurity.Models;

namespace AspNetApiSecurity.Controllers
{
    [Authorize]
    [RoutePrefix("api/account")]
    public class ApiAccountController : ApiController
    {
        public ApiAccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public ApiAccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        private IAuthenticationManager AuthManager { 
            get
            {
                if(Request != null)
                {
                    return Request.GetOwinContext().Authentication;
                }
                else
                {
                    return null;
                }
            }
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        [HttpGet]
        [AllowAnonymous]
        [Route("ping")]
        public IHttpActionResult PingMe()
        {
            return Ok(string.Format("{0} {1:hh:mm:ss}", "It's alive! And the current time is", DateTime.Now));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IHttpActionResult> PostLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindAsync(model.UserName, model.Password);
            if (user != null)
            {
                var success = await SignInAsync(user, model.RememberMe);
                if(success)
                {
                    return Ok(model);
                }
                ModelState.AddModelError("", "Error while identifying user.");
                return BadRequest(ModelState);
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return BadRequest(ModelState);

        }

        private async Task<bool> SignInAsync(ApplicationUser user, bool isPersistent)
        {
            if(AuthManager == null)
            {
                return false;
            }

            AuthManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            return true;
        }


    }
}