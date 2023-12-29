using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using WordPressPCL.Models;

namespace RyanSwanstrom.Function
{
    public class LinkedIn
    {
        // An Azure Function to deploy WordPress Blog Post Content to LinkedIn
        // time hour 14 in UTC is 8am CST
        // TIMER_SCHEDULE is a binding expression, every 2 minutes on local or 8:30am on prod
        [FunctionName("LinkedIn")]
        public void Run([TimerTrigger("%TIMER_SCHEDULE%")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"LinkedIn: checking for new blog posts: {DateTime.Now}");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.LINKEDIN_CAT, 1.0, log);

            foreach (SocialPost post in posts)
            {
                PostToLinkedIn(post, log);
            }                        
            log.LogInformation($"LinkedIn: finished writing {posts.Count} blog posts to social media");
        }
        
        public static void PostToLinkedIn(SocialPost post, ILogger log)
        {            
            log.LogInformation($"PostToLinkedIn starting");
            string platform = "linkedin";
            string response = "";
            if (post != null && !string.IsNullOrEmpty(post.Text))
            {
                JsonObject json = new JsonObject();
                json.Add("post", (post.Text.Length > 3000) ? post.Text.Substring(0, 2999) : post.Text);

                JsonArray platforms = new JsonArray();
                platforms.Add(platform);
                json.Add("platforms", platforms);

                if ( !String.IsNullOrEmpty(post.Video) )
                {
                    JsonArray mediaUrls = new JsonArray();
                    mediaUrls.Add(post.Video); // set to the Photo URL
                    json.Add("mediaUrls", mediaUrls);

                    if ( !String.IsNullOrEmpty(post.VideoThumbnail))
                    {
                        JsonObject options = new JsonObject();
                        options.Add("thumbNail", post.VideoThumbnail);
                        json.Add("linkedInOptions", options); 
                    }
                }

                //add auto schedule options
                JsonObject sched = new JsonObject();
                sched.Add("title", SocialMediaHelper.LINKEDIN_SCHEDULE);
                sched.Add("schedule", true);
                json.Add("autoSchedule", sched); 

                response = SocialMediaHelper.PostToSocial(json, log);
            }
            else
            {
                log.LogInformation($"PostToLinkedIn: Text is null: {post}");
            }
            log.LogInformation($"PostToLinkedIn complete - response: {response}");
        }
    }
}
