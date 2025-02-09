using GestioneAccessi.Web.SignalR;
using Microsoft.Extensions.DependencyInjection;
using GestioneAccessi.Services.Shared;

namespace GestioneAccessi.Web
{
    public class Container
    {
        public static void RegisterTypes(IServiceCollection container)
        {
            // Registration of all the database services you have
            container.AddScoped<SharedService>();

            // Registration of SignalR events
            container.AddScoped<IPublishDomainEvents, SignalrPublishDomainEvents>();
        }
    }
}
