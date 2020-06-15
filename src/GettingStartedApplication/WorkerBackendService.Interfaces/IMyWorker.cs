namespace WorkerBackendService.Interfaces
{
    using System.Threading.Tasks;
    using ActorBackendService.Interfaces;
    using Microsoft.ServiceFabric.Actors;

    public interface IMyWorker : IActor
    {
        Task StartProcessingAsync(string task, IMyActor parent);
    }
}