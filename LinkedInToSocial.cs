using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class LinkedInToSocial
    {
        // An Azure Function to deploy WordPress Blog Post Content to LinkedIn
        [FunctionName("LinkedInToSocial")]
        public void Run([TimerTrigger("17 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation($"Eventually, this Azure Function will check the blog and post new stuff to LinkedIn");
            log.LogInformation($"More Posting to LinkedIn");
        }
    }
}
