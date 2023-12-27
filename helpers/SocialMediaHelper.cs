
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;

namespace RyanSwanstrom.Function
{
    public class SocialMediaHelper
    {
        private static string AYRSHARE_API_KEY = Environment.GetEnvironmentVariable("AYRSHARE_API_KEY");
        private static string WILL_POST_TO_SOCIAL = Environment.GetEnvironmentVariable("WILL_POST_TO_SOCIAL");
       
        public static string PostToSocial(JsonObject json)
        {
            Console.WriteLine($"PostToSocial: {json.ToString()}");
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
                Console.WriteLine("PostToSocial: No results because WILL_POST_TO_SOCIAL flag is not set to 'y'");
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