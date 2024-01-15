using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
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
                PostPinterest(post, log);
            }                        
            log.LogInformation($"Pinterest: finished writing {posts.Count} blog posts to social media");
        }

        public static void PostPinterest(SocialPost post, ILogger log)
        {
            string platform = "pinterest";
            log.LogInformation($"Post {platform} starting");

            if (String.IsNullOrEmpty(post.Video) )
            {
                log.LogInformation($"Pinterest: post does not contain a video");
                return;
            }
 
            string postText = String.Empty;
            if (!String.IsNullOrEmpty(post.Text))
            {
                postText = post.Text;
            }     

            JsonObject json = new JsonObject();
            json.Add("post", postText[..Math.Min(postText.Length, 500)]);

            JsonArray platforms = new JsonArray();
            platforms.Add(platform);
            json.Add("platforms", platforms);

            JsonArray mediaUrls = new JsonArray();
            mediaUrls.Add(post.Video); // set to the video URL
            json.Add("mediaUrls", mediaUrls);

            // add Pinterest options
            //add pinterest options
            JsonObject pinOptions = new JsonObject();
            if (!String.IsNullOrEmpty(post.Title))
            {
                pinOptions.Add("title", post.Title[..Math.Min(post.Title.Length, 100)]); 
            }            
            pinOptions.Add("link", post.URL);          
            if (!String.IsNullOrEmpty(post.VideoThumbnail))
            {
                pinOptions.Add("thumbNail", post.VideoThumbnail);
            }  
            json.Add("pinterestOptions", pinOptions);
            
            //add auto schedule options
            JsonObject sched = new JsonObject();
            sched.Add("title", SocialMediaHelper.PINTEREST_SCHEDULE);
            sched.Add("schedule", true);
            json.Add("autoSchedule", sched);

            string response = SocialMediaHelper.PostToSocial(json, log);
            
            log.LogInformation($"Post {platform} response: {response}");
        }
    }
}
