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
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public IActionResult SignUp()
        {
            var model = new SignUpModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _pool.GetUser(model.Email);
            if (user.Status != null)
            {
                ModelState.AddModelError("User Exists", "User with this email already exists.");
                return View();
            }

            user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
            var createdUser = await _userManager.CreateAsync(user, model.Password);

            if (createdUser.Succeeded)
                return RedirectToAction("Confirm");

            return View(model);
        }
        public IActionResult Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("NotFound", "A user with given email address was not found.");
                return View(model);
            }

            var result = await ((CognitoUserManager<CognitoUser>)_userManager).ConfirmSignUpAsync(user, model.Code, true);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return View(model);
        }

        public IActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("NotFound", "A user or password invalid.");
            return View(model);
        }

        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return View(model);

            await user.ForgotPasswordAsync();

            return RedirectToAction("ResetPassword", "Accounts");
        }

        public IActionResult ResetPassword()
        {
            var model = new ResetPasswordModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return View(model);

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            return View(model);
        }
    }
}
