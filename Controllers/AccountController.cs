using ELMS.Data;
using ELMS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace ELMS.Controllers
{
    public class AccountController : Controller
    {
        private ELMSDbContext _ELMScontext;
        public AccountController(ELMSDbContext ELMScontext)
        {
            _ELMScontext = ELMScontext;
        }

        public ELMSDbContext ELMScontext { get; }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var employee = await _ELMScontext.Employees
                    .FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
                if (employee != null)
                {
                    ClaimsIdentity EmployeeIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    EmployeeIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, ClaimTypes.Email));
                    EmployeeIdentity.AddClaim(new Claim(ClaimTypes.Name, employee.Name));
                    EmployeeIdentity.AddClaim(new Claim(ClaimTypes.Email, employee.Email));
                    EmployeeIdentity.AddClaim(new Claim(ClaimTypes.Role, employee.RoleId.ToString()));
                    EmployeeIdentity.AddClaim(new Claim("Id", employee.Id.ToString()));

                    var principal = new ClaimsPrincipal(EmployeeIdentity);

                    var authenticationProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                        ExpiresUtc = DateTime.Now.AddHours(5),
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                       principal,
                       authenticationProperties);

                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Email), "Email or password is incorrect");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

    }
}

