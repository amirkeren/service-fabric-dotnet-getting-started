namespace WebService.Controllers
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;
    using ActorBackendService.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    [Route("api/[controller]")]
    public class ActorBackendServiceController: Controller
    {
        private readonly FabricClient fabricClient;
        private readonly ConfigSettings configSettings;
        private readonly StatelessServiceContext serviceContext;

        public ActorBackendServiceController(StatelessServiceContext serviceContext, ConfigSettings configSettings, FabricClient fabricClient)
        {
            this.serviceContext = serviceContext;
            this.configSettings = configSettings;
            this.fabricClient = fabricClient;
        }

        // GET api/actorbackendservice
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery(Name = "incidentId")] int incidentId)
        {
            IMyActor proxy = ActorProxy.Create<IMyActor>(new ActorId(incidentId));
            await proxy.SetTaskAsync(Guid.NewGuid().ToString());
            return Json(true);
        }
    }
}