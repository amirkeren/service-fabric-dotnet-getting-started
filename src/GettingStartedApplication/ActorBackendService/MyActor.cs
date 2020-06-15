namespace ActorBackendService
{
    using System.Collections;
    using System.Threading.Tasks;
    using ActorBackendService.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using WorkerBackendService.Interfaces;

    [StatePersistence(StatePersistence.Persisted)]
    internal class MyActor: Actor, IMyActor
    {
        private const string QueueStateName = "tasks";
        private const string RunneStateName = "runner";

        public MyActor(ActorService actorService, ActorId actorId): base(actorService, actorId) {}

        public async Task SetTaskAsync(string task)
        {
            var taskQueue = await StateManager.GetStateAsync<Queue>(QueueStateName);
            taskQueue.Enqueue(task);
            await StateManager.SetStateAsync(QueueStateName, taskQueue);
            if (!await StateManager.ContainsStateAsync(RunneStateName))
            {
                var myWorker = ActorProxy.Create<IMyWorker>(ActorId.CreateRandom());
                await StateManager.SetStateAsync(RunneStateName, myWorker);
                await myWorker.StartProcessingAsync(task, this);
            }
        }

        public async Task GetNextTaskAsync()
        {
            var taskQueue = await StateManager.GetStateAsync<Queue>(QueueStateName);
            taskQueue.Dequeue(); // task was complete, remove it from queue
            var nextTask = taskQueue.Peek() as string;
            var myWorker = await StateManager.GetStateAsync<IMyWorker>(RunneStateName);
            await myWorker.StartProcessingAsync(nextTask, this);
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            await StateManager.SetStateAsync(QueueStateName, new Queue());
            await base.OnActivateAsync();
        }
    }
}