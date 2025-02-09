using Microsoft.AspNetCore.Mvc;
using GestioneAccessi.Web.Data;
using System.Linq;

namespace GestioneAccessi.Web.Areas.Visite.Controllers
{
    [Area("Visite")]
    public partial class StampaController : Controller
    {
        private readonly IngressoEventRepository _repository;

        public StampaController(IngressoEventRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual IActionResult Index()
        {
            var ingressiOggi = _repository.GetIngressiOggi()
                .Select((e, index) => $"{index + 1}) {e.GetType().GetProperty("Nome").GetValue(e)} {e.GetType().GetProperty("Cognome").GetValue(e)} ({e.GetType().GetProperty("Azienda").GetValue(e)})")
                .ToList();

            return View("StampaLive", ingressiOggi);
        }
    }
}
