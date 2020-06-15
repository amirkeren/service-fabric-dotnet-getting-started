namespace WorkerBackendService
{
    using System;
    using System.Threading.Tasks;
    using ActorBackendService.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using WorkerBackendService.Interfaces;

    [StatePersistence(StatePersistence.Persisted)]
    internal class MyWorker: Actor, IMyWorker, IRemindable
    {
        private const string ReminderName = "taskReminder";
        private const string ParentName = "actorParent";
        private const string StateName = "taskRun";

        public MyWorker(ActorService actorService, ActorId actorId): base(actorService, actorId) {}

        public async Task StartProcessingAsync(string task, IMyActor parent)
        {
            await StateManager.SetStateAsync(StateName, task);
            await StateManager.SetStateAsync(ParentName, parent);
            await RegisterReminderAsync(ReminderName, null, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName.Equals(ReminderName, StringComparison.OrdinalIgnoreCase))
            {
                await Task.Delay(TimeSpan.FromSeconds(10)); // simulating a long running task
                var parentActor = await StateManager.GetStateAsync<IMyActor>(ParentName);
                await parentActor.GetNextTaskAsync();
            }
        }

        protected override async Task OnActivateAsync()
        {
            WorkerEventSource.Current.ActorMessage(this, "Worker activated.");
            await base.OnActivateAsync();
        }
    }
}