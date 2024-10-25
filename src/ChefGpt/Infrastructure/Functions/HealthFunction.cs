using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChefGpt.Infrastructure.Functions
{
    public class HealthFunction
    {
        private readonly ILogger<HealthFunction> logger;

        public HealthFunction(ILogger<HealthFunction> logger)
        {
            this.logger = logger;
        }

        [Function("Health")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            this.logger.LogInformation("Health Function: Http Triggered");
            return new OkObjectResult("OK");
        }
    }
}
