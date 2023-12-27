using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class TikTok
    {
        // An Azure Function to deploy WordPress Blog Post Content to TikTok
        // time hour 14 in UTC is 8am CST
        [FunctionName("TikTok")]
        public void Run([TimerTrigger("17 4 14 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"TikTok");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation($"This Azure Function will check the blog and post new vids to TikTok");
        }
    }
}
