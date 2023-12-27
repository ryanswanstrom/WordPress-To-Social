using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class LinkedIn
    {
        private static string LINKEDIN_SCHEDULE = Environment.GetEnvironmentVariable("LINKEDIN_SCHEDULE");

        // An Azure Function to deploy WordPress Blog Post Content to LinkedIn
        // time hour 14 in UTC is 8am CST
        [FunctionName("LinkedIn")]
        public void Run([TimerTrigger("17 30 14 * * *")]TimerInfo myTimer, ILogger log)
        {            
            log.LogInformation($"LinkedIn: checking for new blog posts: {DateTime.Now}");
            log.LogInformation($"This Azure Function will check the blog and post new stuff to LinkedIn");
            List<SocialPost> posts = BlogHelper.FindPosts(BlogHelper.LINKEDIN_CAT, 1.0);


            foreach (SocialPost post in posts)
            {
                PostToLinkedIn(post);
            }
                        
            log.LogInformation($"LinkedIn: wrote {posts.Count} blog posts to social media");
        }
        
        public static void PostToLinkedIn(SocialPost post)
        {
            // schedule the posts
            // create the json

            string platform = "linkedin";
            Console.WriteLine($"PostToLinkedIn starting ");
            string response = "";
            if (post != null && !string.IsNullOrEmpty(post.Text))
            {
                JsonObject json = new JsonObject();
                json.Add("post", (post.Text.Length > 3000) ? post.Text.Substring(0, 2999) : post.Text);

                JsonArray platforms = new JsonArray();
                platforms.Add(platform);
                json.Add("platforms", platforms);

                //JsonArray mediaUrls = new JsonArray();
                //mediaUrls.Add(Photo.PhotoUrl); // set to the Photo URL
                //json.Add("mediaUrls", mediaUrls);

                //add auto schedule options
                JsonObject sched = new JsonObject();
                sched.Add("title", LINKEDIN_SCHEDULE);
                sched.Add("schedule", true);
                json.Add("autoSchedule", sched); 

                response = SocialMediaHelper.PostToSocial(json);
            }
            else
            {
                Console.WriteLine($"PostToLinkedIn: Text is null: {post}");
            }
            Console.WriteLine($"PostToLinkedIn complete - response: {response}");
        }
    }
}
