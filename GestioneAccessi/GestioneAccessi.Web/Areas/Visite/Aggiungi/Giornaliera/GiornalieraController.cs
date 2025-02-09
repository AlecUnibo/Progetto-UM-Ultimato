using GestioneAccessi.Web.Data;
using GestioneAccessi.Web.SignalR;
using GestioneAccessi.Web.SignalR.Hubs;
using GestioneAccessi.Web.SignalR.Hubs.Events;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GestioneAccessi.Web.Areas.Visite.Controllers
{
    [Area("Visite")]
    public partial class GiornalieraController : Controller
    {
        private readonly IPublishDomainEvents _publisher;
        private readonly IngressoEventRepository _repository;

        // Iniettiamo il repository tramite il costruttore
        public GiornalieraController(IPublishDomainEvents publisher, IngressoEventRepository repository)
        {
            _publisher = publisher;
            _repository = repository;
        }

        [HttpGet]
        public virtual IActionResult Index()
        {
            return View("Giornaliera");
        }

        [HttpPost]
        public virtual async Task<IActionResult> EventoGiornaliero(string nome, string cognome, string azienda, string? orarioEntrata, string? orarioUscita)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cognome) || string.IsNullOrWhiteSpace(azienda))
            {
                ModelState.AddModelError("", "Tutti i campi obbligatori devono essere compilati.");
                return View("Pianifica");
            }

            string orarioUscitaFinale = string.IsNullOrWhiteSpace(orarioUscita) ? "-" : orarioUscita;
            string orarioEntrataFinale = string.IsNullOrWhiteSpace(orarioEntrata) ? "-" : orarioEntrata;

            var nuovoEvento = new IngressoAd
            {
                Nome = nome,
                Cognome = cognome,
                Azienda = azienda,
                Data = DateTime.Now,
                OrarioIngresso = orarioEntrataFinale,
                OrarioUscita = orarioUscitaFinale
            };

            _repository.Add(nuovoEvento);
            await _publisher.Publish(nuovoEvento); // Pubblica il nuovo evento

            TempData["SuccessMessage"] = "successMessage";

            return RedirectToAction(nameof(Index));
        }

    }
}
