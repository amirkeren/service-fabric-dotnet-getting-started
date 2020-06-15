// ------------------------------------------------------------
namespace ActorBackendService.Interfaces
{
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;

    public interface IMyActor : IActor
    {
        Task SetTaskAsync(string task);

        Task GetNextTaskAsync();
    }
}