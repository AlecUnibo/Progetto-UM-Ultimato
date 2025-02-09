using GestioneAccessi.Web.Data;
using Microsoft.AspNetCore.Mvc;
using GestioneAccessi.Web.SignalR;
using GestioneAccessi.Web.SignalR.Hubs.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneAccessi.Web.Areas.Visite.Controllers
{
    [Area("Visite")]
    public partial class StoricoController : Controller
    {
        private readonly IngressoEventRepository _repository;

        public StoricoController(IngressoEventRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual IActionResult Index()
        {
            return View("Storico");
        }
        [HttpPost]
        public virtual async Task<IActionResult> FiltraIngressi([FromBody] FiltraIngressiRequest request)
        {
            var (ingressiEvent, ingressiAd) = await _repository.FiltraIngressi(
                request.Data, request.Entrata, request.Uscita, request.Nome, request.Cognome, request.Azienda
            );
            return Json(new { ingressiEvent, ingressiAd });
        }
        public class FiltraIngressiRequest
        {
            public string? Data { get; set; }
            public string? Entrata { get; set; }
            public string? Uscita { get; set; }
            public string? Nome { get; set; }
            public string? Cognome { get; set; }
            public string? Azienda { get; set; }
        }

    }
}
