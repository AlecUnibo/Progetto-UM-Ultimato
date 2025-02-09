using System.Threading.Tasks;

namespace GestioneAccessi.Web.SignalR
{
    public interface IPublishDomainEvents
    {
        Task Publish(object evnt);
    }
}
