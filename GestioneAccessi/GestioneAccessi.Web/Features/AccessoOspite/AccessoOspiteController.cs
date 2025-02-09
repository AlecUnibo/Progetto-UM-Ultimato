using GestioneAccessi.Web.Data;
using GestioneAccessi.Web.SignalR;
using GestioneAccessi.Web.SignalR.Hubs;
using GestioneAccessi.Web.SignalR.Hubs.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneAccessi.Web.Features.AccessoOspite
{
    public partial class AccessoOspiteController : Controller
    {
        private const string StatoAccessoCookie = "StatoAccesso";
        private readonly IPublishDomainEvents _publisher;
        private readonly IngressoEventRepository _repository;

        public AccessoOspiteController(IPublishDomainEvents publisher, IngressoEventRepository repository)
        {
            _publisher = publisher;
            _repository = repository;
        }

        [HttpGet]
        public virtual IActionResult Index()
        {
            var statoAccesso = Request.Cookies[StatoAccessoCookie];

            if (string.IsNullOrEmpty(statoAccesso) || statoAccesso == "Ingresso")
            {
                return RedirectToAction(nameof(Ingresso));
            }

            return RedirectToAction(nameof(Uscita));
        }

        [HttpGet]
        public virtual IActionResult Ingresso()
        {
            ViewData["Title"] = "Pagina di Ingresso";
            return View();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Ingresso(string Nome, string Cognome, string Azienda)
        {
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Cognome) || string.IsNullOrWhiteSpace(Azienda))
            {
                ModelState.AddModelError(string.Empty, "Tutti i campi sono obbligatori.");
                return View();
            }

            var ingressoEvent = new IngressoEvent
            {
                Nome = Nome,
                Cognome = Cognome,
                Azienda = Azienda,
                OrarioIngresso = DateTime.Now.ToString("HH:mm")
            };

            _repository.Add(ingressoEvent); // Salva l'evento nel repository
            await _publisher.Publish(ingressoEvent);

            Response.Cookies.Append(StatoAccessoCookie, "Uscita", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(3),
                HttpOnly = true
            });

            

            return RedirectToAction(nameof(CheckIn));
        }

        [HttpGet]
        public virtual IActionResult CheckIn()
        {
            return View();
        }

        [HttpGet]
        public virtual IActionResult Uscita()
        {
            ViewData["Title"] = "Pagina di Uscita";
            return View("Uscita");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UscitaPost()
        {
            var uscitaEvent = new UscitaEvent
            {
                OrarioUscita = DateTime.Now.ToString("HH:mm"),
            };

            await _publisher.Publish(uscitaEvent);

            Response.Cookies.Delete(StatoAccessoCookie);
            return RedirectToAction(nameof(CheckOut));
        }

        [HttpGet]
        public virtual IActionResult CheckOut()
        {
            return View();
        }

        [HttpGet]
        public virtual IActionResult PaginaBlu()
        {
            return View();
        }

    }
}
