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
            if (String.IsNullOrEmpty(post.Video) )
            {
                log.LogInformation($"YouTube: post does not contain a video");
                return;
            } 
            if (String.IsNullOrEmpty(post.Text))
            {
                log.LogInformation($"YouTube: post does not contain Text");
                return;
            }

            if (!post.IsVideoVertical) 
            {
                post.Text = post.Text + Environment.NewLine + Environment.NewLine;
                post.Text = post.Text + "*Affiliate Links: As an Amazon Associate I earn from qualifying purchases.*" + Environment.NewLine;
                post.Text = post.Text + "Original Blog Post: " + post.URL;
            }

            JsonObject json = new JsonObject();
            json.Add("post", post.Text[..Math.Min(post.Text.Length, 3000)]);

            JsonArray platforms = new JsonArray();
            platforms.Add(platform);
            json.Add("platforms", platforms);

            JsonArray mediaUrls = new JsonArray();
            mediaUrls.Add(post.Video); // set to the vid URL
            json.Add("mediaUrls", mediaUrls);

            //add youtube options
            JsonObject ytOptions = new JsonObject();
            if (!String.IsNullOrEmpty(post.Title)) 
            {
                ytOptions.Add("title", post.Title[..Math.Min(post.Title.Length, 100)] );
            }
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

            string response = SocialMediaHelper.PostToSocial(json, log);
            
            log.LogInformation($"Post {platform} response: {response}");
        }
    }
}
