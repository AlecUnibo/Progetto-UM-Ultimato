using GestioneAccessi.Web.SignalR.Hubs.Events;
using GestioneAccessi.Web.SignalR;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

public interface ITemplateClientEvent
{
    Task NewMessage(Guid idUser, Guid idMessage);
    Task NewIngresso(IngressoEvent ingressoEvent);
    Task NewUscita(UscitaEvent uscitaEvent);
    Task NewIngressoAd(IngressoAd ingressoAd); // Metodo per il nuovo evento
}

public class TemplateHub : Hub<ITemplateClientEvent>
{
    private readonly IPublishDomainEvents _publisher;

    public TemplateHub(IPublishDomainEvents publisher)
    {
        _publisher = publisher;
    }

    public async Task JoinGroup(Guid idGroup)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, idGroup.ToString());
    }

    public async Task LeaveGroup(Guid idGroup)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, idGroup.ToString());
    }
}
