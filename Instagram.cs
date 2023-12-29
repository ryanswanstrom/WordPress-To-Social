using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class Instagram
    {
        // An Azure Function to deploy WordPress Blog Post Content to Instagram
        // time hour 14 in UTC is 8am CST
        [FunctionName("Instagram")]
        public void Run([TimerTrigger("%TIMER_SCHEDULE%")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"Instagram: checking for new blog posts: {DateTime.Now}");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.IG_CAT, 1.0, log);

            foreach (SocialPost post in posts)
            {
                //Post(post, log);
            }                        
            log.LogInformation($"Instagram: finished writing {posts.Count} blog posts to social media");
        }
    }
}
