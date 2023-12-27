using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class LinkedIn
    {
        // An Azure Function to deploy WordPress Blog Post Content to LinkedIn
        // time hour 14 in UTC is 8am CST
        [FunctionName("LinkedIn")]
        public void Run([TimerTrigger("17 30 14 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"LinkedIn");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation($"This Azure Function will check the blog and post new stuff to LinkedIn");
        }
    }
}
