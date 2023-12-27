using System;
using System.Collections.Generic;
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
        public void Run([TimerTrigger("17 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"LinkedIn: checking for new blog posts: {DateTime.Now}");
            log.LogInformation($"This Azure Function will check the blog and post new stuff to LinkedIn");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.LINKEDIN_CAT, 1.0);
            
            log.LogInformation($"LinkedIn: total number of blog posts: {posts.Count}");
        }
    }
}
