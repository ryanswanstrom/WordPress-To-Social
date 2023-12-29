using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class YouTube
    {
        // An Azure Function to deploy WordPress Blog Post Content to YouTube
        // time hour 14 in UTC is 8am CST
        [FunctionName("YouTube")]
        public void Run([TimerTrigger("%TIMER_SCHEDULE%")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"YouTube: checking for new blog posts: {DateTime.Now}");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.YOUTUBE_CAT, 1.0, log);

            foreach (SocialPost post in posts)
            {
                PostToYouTube(post, log);
            }                        
            log.LogInformation($"YouTube: finished writing {posts.Count} blog posts to social media");
        }

        public static void PostToYouTube(SocialPost post, ILogger log)
        {
            string platform = "youtube";
            log.LogInformation($"Post {platform} starting");
            string response = "";
            if (post.Video != null && !string.IsNullOrEmpty(post.Title) && !string.IsNullOrEmpty(post.Text))
            {

                JsonObject json = new JsonObject();
                json.Add("post", post.Text);

                JsonArray platforms = new JsonArray();
                platforms.Add(platform);
                json.Add("platforms", platforms);

                JsonArray mediaUrls = new JsonArray();
                mediaUrls.Add(post.Video); // set to the vid URL
                json.Add("mediaUrls", mediaUrls);

                //add youtube options
                JsonObject ytOptions = new JsonObject();
                ytOptions.Add("title", post.Title);
                ytOptions.Add("visibility", "public");
                if (!String.IsNullOrEmpty(post.VideoThumbnail))
                {
                    ytOptions.Add("thumbNail", post.VideoThumbnail);
                }
                if (post.IsVideoVertical)
                {
                    ytOptions.Add("shorts", true);
                }
                ytOptions.Add("notifySubscribers", true);
                JsonArray jsonTags = new JsonArray();
                foreach (string tag in post.Tags)
                {
                    jsonTags.Add(tag);
                }
                ytOptions.Add("tags", jsonTags);
                json.Add("youTubeOptions", ytOptions);

                
                //add auto schedule options
                JsonObject sched = new JsonObject();
                sched.Add("title", SocialMediaHelper.YOUTUBE_SCHEDULE);
                sched.Add("schedule", true);
                json.Add("autoSchedule", sched); 

                response = SocialMediaHelper.PostToSocial(json, log);
            }
            else
            {
                log.LogInformation($"Post {platform}: Video, title, or desc is null: {post}");
            }
            log.LogInformation($"Post {platform} response: {response}");
        }
    }
}
