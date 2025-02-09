using System;
using System.Globalization;

namespace GestioneAccessi.Web.SignalR.Hubs.Events
{
    public class NewMessageEvent
    {
        public Guid IdGroup { get; set; }
        public Guid IdUser { get; set; }
        public Guid IdMessage { get; set; }
    }

    public class IngressoEvent
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Azienda { get; set; }
        public string OrarioIngresso { get; set; }
        public string OrarioUscita { get; set; } = "-";
        public DateTime Data { get; set; } = DateTime.Now;
    }

    public class UscitaEvent
    {
        public int Id { get; set; }
        public string OrarioUscita { get; set; }  
    }

    public class IngressoAd
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Azienda { get; set; }
        public string OrarioIngresso { get; set; }
        public string OrarioUscita { get; set; } = "-";
        public DateTime Data { get; set; }
    }
}
