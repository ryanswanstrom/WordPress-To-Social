using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
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
            string platform = "tiktok";
            log.LogInformation($"Post {platform} starting");
            if (String.IsNullOrEmpty(post.Video) )
            {
                log.LogInformation($"TikTok: post does not contain a video");
                return;
            } 
            if (!post.IsVideoVertical)
            {                
                log.LogInformation($"TikTok: post does not contain a vertical video");
                return;
            }

            string postText = String.Empty;
            if (!String.IsNullOrEmpty(post.Title))
            {
                postText = post.Title + System.Environment.NewLine;
            }       
            foreach (string tag in post.Tags)
            {
                postText += $"#{tag} ";
            }
            
            JsonObject json = new JsonObject();
            json.Add("post", postText[..Math.Min(postText.Length, 2200)] ); // max 2200, can be empty

            JsonArray platforms = new JsonArray();
            platforms.Add(platform);
            json.Add("platforms", platforms);

            JsonArray mediaUrls = new JsonArray();
            mediaUrls.Add(post.Video); // set to the vid URL
            json.Add("mediaUrls", mediaUrls);
            
            //add youtube options
            JsonObject ttOptions = new JsonObject();
            ttOptions.Add("thumbNailOffset", 800);
            json.Add("tikTokOptions", ttOptions);
            
            //add auto schedule options
            JsonObject sched = new JsonObject();
            sched.Add("title", SocialMediaHelper.TIKTOK_SCHEDULE);
            sched.Add("schedule", true);
            json.Add("autoSchedule", sched); 

            string response = SocialMediaHelper.PostToSocial(json, log);
            
            log.LogInformation($"Post {platform} response: {response}");
        }
    }
}
