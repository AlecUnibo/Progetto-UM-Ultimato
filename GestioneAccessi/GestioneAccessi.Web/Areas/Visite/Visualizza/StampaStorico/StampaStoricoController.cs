using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GestioneAccessi.Web.Areas.Visite.Controllers
{
    [Area("Visite")]
    public partial class StampaStoricoController : Controller
    {
        [HttpPost]
        public virtual IActionResult Index([FromBody] List<EventoModel> eventi)
        {
            if (eventi == null || eventi.Count == 0)
            {
                return Json(new { redirectUrl = Url.Action("Index", "Storico") });
            }

            TempData["Eventi"] = JsonConvert.SerializeObject(eventi);

            // Correggi il reindirizzamento
            return Json(new { redirectUrl = Url.Action("Stampa", "StampaStorico", new { area = "Visite" }) });
        }
        [HttpGet]
        public virtual IActionResult Stampa()
        {
            if (TempData["Eventi"] == null)
            {
                return RedirectToAction("Index", "Storico");
            }

            var eventi = JsonConvert.DeserializeObject<List<EventoModel>>(TempData["Eventi"].ToString());
            return View("StampaStorico", eventi);
        }

    }
    public class EventoModel
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Azienda { get; set; }
    }
}
