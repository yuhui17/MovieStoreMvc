using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        public UserAuthenticationController(IUserAuthenticationService userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        //we will create a user with admin right, after that we are going to comment this method because we need only one user in this application
        //public async Task<IActionResult> Register()
        //{
        //    var model = new RegistrationModel{
        //        Email = "admin@gmail.com",
        //        Username = "admin",
        //        Name = "yuhui",
        //        Password = "Admin@123",
        //        PasswordConfirm = "Admin@123",
        //        Role = "Admin"
        //    };
        //    //if you want to register with user, change the to Role="User"

        //    var result = await _userAuthenticationService.RegisterAsync(model);

        //    return Ok(result.Message);
        //}

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userAuthenticationService.LoginAsync(model);
            if(result.StatusCode == 1)
            {
                //login success
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //login failure
                TempData["msg"] = "Could not logged in";
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _userAuthenticationService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
