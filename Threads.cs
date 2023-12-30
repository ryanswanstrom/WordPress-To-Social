using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class Threads
    {
        /*
        // An Azure Function to deploy WordPress Blog Post Content to Threads, awaiting API
        // time hour 14 in UTC is 8am CST
        [FunctionName("Threads")]
        public void Run([TimerTrigger("%TIMER_SCHEDULE%")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"Threads: checking for new blog posts: {DateTime.Now}");
            
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.THREADS_CAT, 1.0, log);

            foreach (SocialPost post in posts)
            {
                //Post(post, log);
            }  
                                  
            log.LogInformation($"Threads: finished writing {posts.Count} blog posts to social media");
        }
        */
    }
}
