using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
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
                PostInstagram(post, log);
            }                        
            log.LogInformation($"Instagram: finished writing {posts.Count} blog posts to social media");
        }

        public static void PostInstagram(SocialPost post, ILogger log)
        {
            string platform = "instagram";
            log.LogInformation($"Post {platform} starting");

            if (String.IsNullOrEmpty(post.Video) )
            {
                log.LogInformation($"Instagram: post does not contain a video");
                return;
            } 
            if (!post.IsVideoVertical)
            {                
                log.LogInformation($"Instagram: post does not contain a vertical video");
                return;
            }
 
            string postText = String.Empty;
            if (!String.IsNullOrEmpty(post.Title))
            {
                postText = post.Title + System.Environment.NewLine;
            }
            if (!String.IsNullOrEmpty(post.Text))
            {
                postText = post.Text + System.Environment.NewLine;
            }            
            foreach (string tag in post.Tags)
            {
                postText += $"#{tag} ";
            }

            JsonObject json = new JsonObject();
            json.Add("post", postText[..Math.Min(postText.Length, 2200)]);

            JsonArray platforms = new JsonArray();
            platforms.Add(platform);
            json.Add("platforms", platforms);

            JsonArray mediaUrls = new JsonArray();
            mediaUrls.Add(post.Video); // set to the video URL
            json.Add("mediaUrls", mediaUrls);

            //add InstaGram options
            JsonObject igOptions = new JsonObject();
            igOptions.Add("reels", true);
            igOptions.Add("shareReelsFeed", true);
            if (!String.IsNullOrEmpty(post.VideoThumbnail))
            {
                igOptions.Add("coverUrl", post.VideoThumbnail);
            }            
            json.Add("instagramOptions", igOptions);
            
            //add auto schedule options
            JsonObject sched = new JsonObject();
            sched.Add("title", SocialMediaHelper.INSTAGRAM_SCHEDULE);
            sched.Add("schedule", true);
            json.Add("autoSchedule", sched);

            string response = SocialMediaHelper.PostToSocial(json, log);
            
            log.LogInformation($"Post {platform} response: {response}");
        }
    }
}
