using System.Security.Cryptography;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._pool = pool;
        }
        [HttpGet]
        public async Task<ActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Signup(SignupModel model)
        {
            if (!ModelState.IsValid)
            { }
            var user = _pool.GetUser(model.Email);

            if (user != null && user.Status != null)
            {
                ModelState.AddModelError("", "User with this email alread exists");
            }
            user.Attributes.Add("email", model.Email);
            var createdUser = await _userManager.CreateAsync(user, model.Password);
            if (createdUser.Succeeded)
            {
               return RedirectToAction("Confirm");
            }
            foreach (var item in createdUser.Errors)
            {
                ModelState.AddModelError(item.Code, item.Description);
            }
            return View(model);
        }

        public async Task<ActionResult> Confirm(ConfirmModel model)
        {
            return View(model);

        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<ActionResult> ConfirmSignup(ConfirmModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            var result = await ((CognitoUserManager<CognitoUser>)_userManager).ConfirmSignUpAsync(user, model.Code, true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }

            }
            return View(model);
        }

        public async Task<ActionResult> Login(SignInModel model)
        {
            return View("Login", model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<ActionResult> LoginPost(SignInModel model)
        {

            if (!ModelState.IsValid) return View("Login", model);
            var user = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (user.Succeeded)
                return RedirectToAction("Index", "Home");
            else
            {
                ModelState.AddModelError("LoginError", "Email or password does not match");
            }
            return View("Login", model);
        }
    }
}
