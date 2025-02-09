using GestioneAccessi.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GestioneAccessi.Infrastructure;
using GestioneAccessi.Services.Shared;

namespace GestioneAccessi.Web.Features.Login
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Alerts]
    [ModelStateToTempData]
    public partial class LoginController : Controller
    {
        public static string LoginErrorModelStateKey = "LoginError";
        private readonly SharedService _sharedService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public LoginController(SharedService sharedService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _sharedService = sharedService;
            _sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public virtual IActionResult Login(string returnUrl)
        {
            if (HttpContext.User != null && HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home"); // Reindirizza all'Index di HomeController
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };

            return View(model);
        }

        [HttpPost]
        public async virtual Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica delle credenziali tramite il servizio
                    var utente = await _sharedService.Query(new CheckLoginCredentialsQuery
                    {
                        Email = model.Email,
                        Password = model.Password,
                    });

                    // Reindirizza all'Index di HomeController se le credenziali sono corrette
                    return RedirectToAction("Index", "Home");
                }
                catch (LoginException e)
                {
                    // Aggiunge l'errore nel ModelState se c'è un problema con le credenziali
                    ModelState.AddModelError(LoginErrorModelStateKey, e.Message);
                    ViewData["LoginError"] = "Incorrect Email or password.";
                }
            }

            // Ritorna alla vista Login se ci sono errori
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Logout()
        {
            // Logout utente
            HttpContext.SignOutAsync();

            // Aggiunge un messaggio di successo e reindirizza a Login
            Alerts.AddSuccess(this, "Utente scollegato correttamente");
            return RedirectToAction("Login", "Login");
        }
    }
}
