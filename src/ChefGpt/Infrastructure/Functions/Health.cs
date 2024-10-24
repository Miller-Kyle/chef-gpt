using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChefGpt.Infrastructure.Functions
{
    public class Health
    {
        private readonly ILogger<Health> logger;

        public Health(ILogger<Health> logger)
        {
            this.logger = logger;
        }

        [Function("Health")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            this.logger.LogInformation("Health Funciton: Http Triggered");
            return new OkObjectResult("OK");
        }
    }
}
