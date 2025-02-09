using GestioneAccessi.Web.Data;
using GestioneAccessi.Web.SignalR.Hubs;
using GestioneAccessi.Web.SignalR.Hubs.Events;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace GestioneAccessi.Web.SignalR
{
    public class SignalrPublishDomainEvents : IPublishDomainEvents
    {
        private readonly IHubContext<TemplateHub, ITemplateClientEvent> _templateHub;
        private readonly IngressoEventRepository _repository; // Aggiungi il repository per IngressoEvent
        private static int _currentIngressoId = 0; // Contatore statico per gli eventi di ingresso
        private static int _currentUscitaId = 0;   // Contatore statico per gli eventi di uscita

        // Aggiungi il repository come dipendenza nel costruttore
        public SignalrPublishDomainEvents(IHubContext<TemplateHub, ITemplateClientEvent> templateHub, IngressoEventRepository repository)
        {
            _templateHub = templateHub;
            _repository = repository;
        }

        private ITemplateClientEvent GetTemplateGroup(Guid id)
        {
            return _templateHub.Clients.Group(id.ToString());
        }

        public Task Publish(object evnt)
        {
            try
            {
                Console.WriteLine($"Pubblicazione evento: {evnt.GetType().Name}");
                return ((dynamic)this).When((dynamic)evnt);
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                return Task.CompletedTask;
            }
        }

        public Task When(NewMessageEvent e)
        {
            return GetTemplateGroup(e.IdGroup).NewMessage(e.IdUser, e.IdMessage);
        }

        public async Task When(IngressoEvent e)
        {
            _currentIngressoId++; // Incrementa l'ID per ogni evento Ingresso
            e.Id = _currentIngressoId; // Imposta l'ID
            Console.WriteLine($"Evento Ingresso ricevuto: Id={e.Id}, Nome={e.Nome}, Cognome={e.Cognome}, Azienda={e.Azienda}, Data={e.Data}, Entrata={e.OrarioIngresso}, Uscita= {e.OrarioUscita}");
            // Pubblica a tutti i client
            try
            {
                await _templateHub.Clients.All.NewIngresso(e); // Aspetta il completamento
                Console.WriteLine("Evento pubblicato correttamente a tutti i client.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante la pubblicazione dell'evento:");
                if (ex is AggregateException aggregateException)
                {
                    foreach (var innerException in aggregateException.InnerExceptions)
                    {
                        Console.WriteLine(innerException.Message);
                    }
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task When(UscitaEvent e)
        {
            _currentUscitaId++; // Incrementa l'ID per ogni evento Uscita
            e.Id = _currentUscitaId; // Imposta l'ID
            Console.WriteLine($"Evento Uscita ricevuto: Id={e.Id}, Uscita= {e.OrarioUscita}");

            // Trova l'evento di ingresso corrispondente (basato sull'ID dell'evento di uscita)
            var ingressoEvent = _repository.GetAll().FirstOrDefault(i => i.Id == e.Id);
            if (ingressoEvent != null)
            {
                ingressoEvent.OrarioUscita = e.OrarioUscita; // Aggiorna l'ora di uscita dell'ingresso
                _repository.Update(ingressoEvent); // Aggiorna il repository
                Console.WriteLine($"Orario di uscita dell'ingresso aggiornato: {ingressoEvent.OrarioUscita}");
            }

            // Pubblica l'evento di uscita a tutti i client
            await _templateHub.Clients.All.NewUscita(e);
        }

        public async Task When(IngressoAd e)
        {
            Console.WriteLine($"Evento IngressoAd ricevuto: Nome={e.Nome}, Cognome={e.Cognome}, Azienda={e.Azienda}, Data={e.Data}, Entrata={e.OrarioIngresso}, Uscita={e.OrarioUscita}");

            // Pubblica l'evento a tutti i client
            try
            {
                await _templateHub.Clients.All.NewIngressoAd(e); // Aspetta il completamento
                Console.WriteLine("Evento IngressoAd pubblicato correttamente a tutti i client.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante la pubblicazione dell'evento:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
