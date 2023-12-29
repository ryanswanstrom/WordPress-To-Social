
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace RyanSwanstrom.Function
{
    public class SocialMediaHelper
    {
        private static string AYRSHARE_API_KEY = Environment.GetEnvironmentVariable("AYRSHARE_API_KEY");
        private static string WILL_POST_TO_SOCIAL = Environment.GetEnvironmentVariable("WILL_POST_TO_SOCIAL");        
        public static string LINKEDIN_SCHEDULE = Environment.GetEnvironmentVariable("LINKEDIN_SCHEDULE");       
        public static string YOUTUBE_SCHEDULE = Environment.GetEnvironmentVariable("YOUTUBE_SCHEDULE");       
        public static string TIKTOK_SCHEDULE = Environment.GetEnvironmentVariable("TIKTOK_SCHEDULE");       
        public static string INSTAGRAM_SCHEDULE = Environment.GetEnvironmentVariable("INSTAGRAM_SCHEDULE");       
        public static string FACEBOOK_SCHEDULE = Environment.GetEnvironmentVariable("FACEBOOK_SCHEDULE");     
        public static string PINTEREST_SCHEDULE = Environment.GetEnvironmentVariable("PINTEREST_SCHEDULE");     
        public static string THREADS_SCHEDULE = Environment.GetEnvironmentVariable("THREADS_SCHEDULE");
       
        public static string PostToSocial(JsonObject json, ILogger log)
        {
            log.LogInformation($"PostToSocial: {json.ToString()}");
            string rVal = string.Empty;

            if ("y".Equals(WILL_POST_TO_SOCIAL))
            {
                string url = "https://app.ayrshare.com/api/post";

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AYRSHARE_API_KEY);
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content = new StringContent(json.ToString(),
                                        Encoding.UTF8,
                                        "application/json")
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage resMess = client.Send(request);

                using (var streamReader = new StreamReader(resMess.Content.ReadAsStream()))
                {
                    rVal = streamReader.ReadToEnd();
                }
            }
            else
            {
                log.LogInformation("PostToSocial: No results because WILL_POST_TO_SOCIAL flag is not set to 'y'");
            }

            return rVal;
        }

        public static string CreateHashTags(List<string> tags)
        {
            string hashtags = " ";
            if (tags != null)
            {
                foreach (string t in tags)
                {
                    hashtags += $"#{t} ";
                }
            }
            return hashtags;
        }
    }

    
}