using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class Facebook
    {
        // An Azure Function to deploy WordPress Blog Post Content to Facebook
        // time hour 14 in UTC is 8am CST
        [FunctionName("Facebook")]
        public void Run([TimerTrigger("%TIMER_SCHEDULE%")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"Facebook: checking for new blog posts: {DateTime.Now}");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.FB_CAT, 1.0, log);

            foreach (SocialPost post in posts)
            {
                PostFacebook(post, log);
            }                        
            log.LogInformation($"Facebook: finished writing {posts.Count} blog posts to social media");
        }
        public static void PostFacebook(SocialPost post, ILogger log)
        {
            string platform = "facebook";
            log.LogInformation($"Post {platform} starting");

            if (String.IsNullOrEmpty(post.Video) )
            {
                log.LogInformation($"Facebook: post does not contain a video");
                return;
            } 
            if (!post.IsVideoVertical)
            {                
                log.LogInformation($"Facebook: post does not contain a vertical video");
                return;
            }
 
            string postText = String.Empty;
            if (!String.IsNullOrEmpty(post.Text))
            {
                postText = post.Text;
            }     

            JsonObject json = new JsonObject();
            json.Add("post", postText[..Math.Min(postText.Length, 2200)]);

            JsonArray platforms = new JsonArray();
            platforms.Add(platform);
            json.Add("platforms", platforms);

            JsonArray mediaUrls = new JsonArray();
            mediaUrls.Add(post.Video); // set to the video URL
            json.Add("mediaUrls", mediaUrls);

            //add Facebook options
            JsonObject fbOptions = new JsonObject();
            if (post.IsVideoVertical) 
            {
                fbOptions.Add("reels", true);
            }            
            if (!String.IsNullOrEmpty(post.Title))
            {
                fbOptions.Add("title", post.Title[..Math.Min(post.Title.Length, 255)]); 
            }            
            if (!String.IsNullOrEmpty(post.VideoThumbnail))
            {
                fbOptions.Add("thumbNail", post.VideoThumbnail);
            }            
            json.Add("faceBookOptions", fbOptions);
            
            //add auto schedule options
            JsonObject sched = new JsonObject();
            sched.Add("title", SocialMediaHelper.FACEBOOK_SCHEDULE);
            sched.Add("schedule", true);
            json.Add("autoSchedule", sched);

            string response = SocialMediaHelper.PostToSocial(json, log);
            
            log.LogInformation($"Post {platform} response: {response}");
        }
    }
}
