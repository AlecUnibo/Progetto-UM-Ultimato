using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestioneAccessi.Web.SignalR.Hubs.Events;
using Microsoft.Extensions.Logging;
using static GestioneAccessi.Web.Features.Home.HomeController;

namespace GestioneAccessi.Web.Data
{
    public class IngressoEventRepository
    {
        private readonly List<IngressoEvent> _ingressi = new List<IngressoEvent>();
        private readonly List<IngressoAd> _ingressiAd = new List<IngressoAd>();

        public void Add(IngressoEvent ingressoEvent)
        {
            _ingressi.Add(ingressoEvent);
        }

        public List<IngressoEvent> GetAll()
        {
            return _ingressi.OrderByDescending(e => e.Data).ToList();
        }
        //cestino
        public bool Delete(int id)
        {
            var evento = _ingressi.FirstOrDefault(e => e.Id == id);
            if (evento != null)
            {
                _ingressi.Remove(evento);
                // Aggiorna gli ID degli eventi successivi
                for (int i = id - 1; i < _ingressi.Count; i++)
                {
                    _ingressi[i].Id -= 1;
                }
                return true;
            }
            return false;
        }
        //evento d'uscita
        public void Update(IngressoEvent ingressoEvent)
        {
            var existingEvent = _ingressi.FirstOrDefault(e => e.Id == ingressoEvent.Id);
            if (existingEvent != null)
            {
                existingEvent.OrarioUscita = ingressoEvent.OrarioUscita; // Aggiorna l'orario di uscita
            }
        }

        public void Add(IngressoAd ingressoAd)
        {
            _ingressiAd.Add(ingressoAd);
        }

        public List<IngressoAd> GetAllIngressoAd()
        {
            return _ingressiAd.OrderByDescending(e => e.Data).ToList();
        }
        //cestino
        public bool DeleteIngressoAd(IngressoAd ingressoAd)
        {
            var evento = _ingressiAd.FirstOrDefault(e =>
                e.Nome == ingressoAd.Nome &&
                e.Cognome == ingressoAd.Cognome &&
                e.Azienda == ingressoAd.Azienda);
            if (evento != null)
            {
                _ingressiAd.Remove(evento);
                return true;
            }
            return false;
        }

        // Metodo per ottenere solo gli ingressi della giornata attuale pagina StampaLive
        public List<object> GetIngressiOggi()
        {
            DateTime oggi = DateTime.Today;
            var ingressiValidi = _ingressi
                .Where(e => e.Data.Date == oggi &&
                            !string.IsNullOrEmpty(e.OrarioIngresso) &&
                            e.OrarioIngresso != "-" &&
                            (e.OrarioUscita == null || e.OrarioUscita == "-"))
                .Select(e => new { e.Nome, e.Cognome, e.Azienda })
                .ToList();
            var ingressiAdValidi = _ingressiAd
                .Where(e => e.Data.Date == oggi &&
                            !string.IsNullOrEmpty(e.OrarioIngresso) &&
                            e.OrarioIngresso != "-" &&
                            (e.OrarioUscita == null || e.OrarioUscita == "-"))
                .Select(e => new { e.Nome, e.Cognome, e.Azienda })
                .ToList();
            var risultato = new List<object>();
            risultato.AddRange(ingressiValidi);
            risultato.AddRange(ingressiAdValidi);
            return risultato;
        }
        //enter-icon, exit-icon
        public bool UpdateOrario(UpdateOrarioRequest request)
        {
            if (request.Type == "IngressoEvent" && request.Id.HasValue)
            {
                var evento = _ingressi.FirstOrDefault(e => e.Id == request.Id.Value);
                if (evento != null)
                {
                    if (!string.IsNullOrEmpty(request.OrarioIngresso))
                        evento.OrarioIngresso = request.OrarioIngresso;
                    if (!string.IsNullOrEmpty(request.OrarioUscita))
                        evento.OrarioUscita = request.OrarioUscita;
                    return true;
                }
            }
            else if (request.Type == "IngressoAd")
            {
                var evento = _ingressiAd.FirstOrDefault(e =>
                    e.Nome == request.Nome &&
                    e.Cognome == request.Cognome &&
                    e.Azienda == request.Azienda);
                if (evento != null)
                {
                    if (!string.IsNullOrEmpty(request.OrarioIngresso))
                        evento.OrarioIngresso = request.OrarioIngresso;
                    if (!string.IsNullOrEmpty(request.OrarioUscita))
                        evento.OrarioUscita = request.OrarioUscita;
                    return true;
                }
            }
            return false;
        }
        public async Task<(List<IngressoEvent> ingressiEvent, List<IngressoAd> ingressiAd)> FiltraIngressi(
        string? data, string? entrata, string? uscita, string? nome, string? cognome, string? azienda)
        {
            return await Task.Run(() =>
            {
                var oggi = DateTime.Today; // Data odierna senza orario
                var eventiQuery = _ingressi.AsQueryable()
                    .Where(e => e.Data < oggi); // Filtra solo ingressi con data precedente a oggi
                var ingressiAdQuery = _ingressiAd.AsQueryable()
                    .Where(e => e.Data < oggi); // Stesso filtro per ingressiAd
                if (!string.IsNullOrEmpty(data))
                {
                    var dataFiltrata = DateTime.Parse(data);
                    eventiQuery = eventiQuery.Where(e => e.Data.Date == dataFiltrata.Date);
                    ingressiAdQuery = ingressiAdQuery.Where(e => e.Data.Date == dataFiltrata.Date);
                }
                if (!string.IsNullOrEmpty(entrata))
                {
                    eventiQuery = eventiQuery.Where(e => e.OrarioIngresso == entrata);
                    ingressiAdQuery = ingressiAdQuery.Where(e => e.OrarioIngresso == entrata);
                }
                if (!string.IsNullOrEmpty(uscita))
                {
                    eventiQuery = eventiQuery.Where(e => e.OrarioUscita == uscita);
                    ingressiAdQuery = ingressiAdQuery.Where(e => e.OrarioUscita == uscita);
                }
                if (!string.IsNullOrEmpty(nome))
                {
                    eventiQuery = eventiQuery.Where(e => e.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
                    ingressiAdQuery = ingressiAdQuery.Where(e => e.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(cognome))
                {
                    eventiQuery = eventiQuery.Where(e => e.Cognome.Equals(cognome, StringComparison.OrdinalIgnoreCase));
                    ingressiAdQuery = ingressiAdQuery.Where(e => e.Cognome.Equals(cognome, StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(azienda))
                {
                    eventiQuery = eventiQuery.Where(e => e.Azienda.Equals(azienda, StringComparison.OrdinalIgnoreCase));
                    ingressiAdQuery = ingressiAdQuery.Where(e => e.Azienda.Equals(azienda, StringComparison.OrdinalIgnoreCase));
                }
                return (eventiQuery.ToList(), ingressiAdQuery.ToList());
            });
        }
    }
}
