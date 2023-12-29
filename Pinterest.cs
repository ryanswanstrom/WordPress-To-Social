using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class Pinterest
    {
        // An Azure Function to deploy WordPress Blog Post Content to Pinterest
        // time hour 14 in UTC is 8am CST
        [FunctionName("Pinterest")]
        public void Run([TimerTrigger("%TIMER_SCHEDULE%")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"Pinterest: checking for new blog posts: {DateTime.Now}");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.PINTEREST_CAT, 1.0, log);

            foreach (SocialPost post in posts)
            {
                //Post(post, log);
            }                        
            log.LogInformation($"Pinterest: finished writing {posts.Count} blog posts to social media");
        }
    }
}
