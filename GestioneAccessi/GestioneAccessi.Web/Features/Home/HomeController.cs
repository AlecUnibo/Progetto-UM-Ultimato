using GestioneAccessi.Web.Data;
using GestioneAccessi.Web.SignalR;
using GestioneAccessi.Web.SignalR.Hubs.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace GestioneAccessi.Web.Features.Home
{
    public partial class HomeController : Controller
    {
        private readonly IngressoEventRepository _repository;

        public HomeController(IngressoEventRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual IActionResult Index()
        {
            return View("Home");
        }

        [HttpPost]
        public virtual IActionResult ChangeLanguageTo(string cultureName)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), Secure = true }
            );

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
        //con filtro data
        [HttpGet("api/ingressi")]
        public virtual IActionResult GetIngressi()
        {
            var ingressi = _repository.GetAll() ?? new List<IngressoEvent>();
            return Json(ingressi);
        }
        //con filtro data
        [HttpGet("api/ingressiAd")]
        public virtual IActionResult GetIngressiAd()
        {
            var ingressi = _repository.GetAllIngressoAd() ?? new List<IngressoAd>();
            return Json(ingressi);
        }
        //cestino
        [HttpDelete("api/ingressi/{id}")]
        public virtual IActionResult DeleteIngresso(int id)
        {
            var success = _repository.Delete(id);
            if (success)
            {
                return Ok(new { message = "Evento eliminato con successo." });
            }

            return NotFound(new { message = "Evento non trovato." });
        }
        //cestino
        [HttpDelete("api/ingressiAd/delete")]
        public virtual IActionResult DeleteIngressoAd([FromBody] IngressoAd ingressoAd)
        {
            if (ingressoAd == null)
            {
                return BadRequest(new { message = "Dati non validi." });
            }

            var success = _repository.DeleteIngressoAd(ingressoAd);
            if (success)
            {
                return Ok(new { message = "IngressoAd eliminato con successo." });
            }

            return NotFound(new { message = "IngressoAd non trovato." });
        }
        //enter-icon, exit-icon
        [HttpPost("api/ingressi/updateOrario")]
        public virtual IActionResult UpdateOrario([FromBody] UpdateOrarioRequest request)
        {
            if (request == null || (string.IsNullOrEmpty(request.OrarioIngresso) && string.IsNullOrEmpty(request.OrarioUscita)))
            {
                return BadRequest(new { message = "Dati non validi." });
            }

            bool updated = _repository.UpdateOrario(request);

            if (updated)
            {
                return Ok(new { message = "Orario aggiornato con successo." });
            }

            return NotFound(new { message = "Evento non trovato." });
        }

        public class UpdateOrarioRequest
        {
            public int? Id { get; set; } // Solo per IngressoEvent
            public string Type { get; set; }
            public string OrarioIngresso { get; set; }
            public string OrarioUscita { get; set; }

            // Solo per IngressoAd
            public string Nome { get; set; }
            public string Cognome { get; set; }
            public string Azienda { get; set; }
        }

    }
}
