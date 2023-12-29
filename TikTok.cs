using System;
using System.Collections.Generic;
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
        public void Run([TimerTrigger("%TIMER_SCHEDULE%")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"TikTok: checking for new blog posts: {DateTime.Now}");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.TIKTOK_CAT, 1.0, log);

            foreach (SocialPost post in posts)
            {
                PostToTikTok(post, log);
            }                        
            log.LogInformation($"TikTok: finished writing {posts.Count} blog posts to social media");
        }
        public static void PostToTikTok(SocialPost post, ILogger log)
        {
        }
    }
}
