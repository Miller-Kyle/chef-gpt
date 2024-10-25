// Copyright (c) 2024 Kyle Miller. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChefGpt.Infrastructure.Functions
{
    /// <summary>
    ///     Azure Function to check the health status of the application.
    /// </summary>
    public class HealthFunction
    {
        private readonly ILogger<HealthFunction> logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HealthFunction" /> class.
        /// </summary>
        /// <param name="logger">The logger for logging information.</param>
        public HealthFunction(ILogger<HealthFunction> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        ///     Runs the health check function.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>An <see cref="IActionResult" /> indicating the health status.</returns>
        [Function("Health")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            this.logger.LogInformation("Health Function: Http Triggered");
            return new OkObjectResult("OK");
        }
    }
}