using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;
using WordPressPCL.Utility;

namespace RyanSwanstrom.Function
{
    public class BlogHelper
    {
        //pass the Wordpress REST API base address as string
        private static string BLOG_ADDRESS = Environment.GetEnvironmentVariable("BLOG_ADDRESS");  
        public static int LINKEDIN_CAT = int.Parse( Environment.GetEnvironmentVariable("LINKEDIN_CAT") );
        
        public static List<SocialPost> FindPosts(int CatID, double DaysBack)
        {
            Console.WriteLine("Reading the Posts");
            WordPressClient wpclient = new WordPressClient(BLOG_ADDRESS);

            List<SocialPost> socialPosts = new List<SocialPost>();
            var queryBuilder = new PostsQueryBuilder();
            queryBuilder.After = DateTime.Now.AddDays(0-DaysBack);
            queryBuilder.Categories = new List<int> { CatID };
            Task<IEnumerable<Post>> posts = wpclient.Posts.QueryAsync(queryBuilder);
            foreach (Post p in posts.Result)
            {
                SocialPost sp = new SocialPost();
                sp.Title = p.Title.Rendered;
                sp.Text = CleanText(p.Content.Rendered);
                sp.URL = p.Link;
                // TODO: FindImages
                // TODO: FindVideos
                socialPosts.Add(sp);
            }

            return socialPosts;
        }

        private static String CleanText(String text)
        {
            Console.WriteLine(text);
            String cleanString = text;
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            int currIx = 0;
            // find paragraphs and extract text
            // continue until <!--more-->
            // is it <p> <ul> or <ol>
            string output = String.Empty;

            do
            {
                cleanString = cleanString.Trim();
                if (cleanString.StartsWith("<p>"))
                {
                    Console.WriteLine("paragraph");
                    // handle a paragraph
                    int end = cleanString.IndexOf("</p>");
                    output += cleanString.Substring(3, cleanString.IndexOf("</p>") - 4);
                    cleanString = cleanString.Substring(end + 4);
                }
                else if (cleanString.StartsWith("<ul>"))
                {
                    Console.WriteLine("unordered list");
                    // handle the unordered list
                    // while <li> exists, pull out the text and add a dash at beginning and a new line at the end
                    string ulstring = cleanString.Substring(4, cleanString.IndexOf("</ul>") - 5);
                    string[] items = ulstring.Split("<li>", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (string item in items)
                    {
                        string s = item.Substring(0, item.Length-5).Trim();
                        output += " - " + s + Environment.NewLine;
                    }
                    // trim off the training newline
                    output = output.Trim();
                    cleanString = cleanString.Substring(cleanString.IndexOf("</ul>") + 5);
                }
                else if (cleanString.StartsWith("<ol>"))
                {
                    Console.WriteLine("ordered list");
                    // handle the ordered list
                    // while <li> exists, pull out the text and add a dash at beginning and a new line at the end
                    string ulstring = cleanString.Substring(4, cleanString.IndexOf("</ol>") - 5);
                    string[] items = ulstring.Split("<li>", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    int i = 1;
                    foreach (string item in items)
                    {
                        string s = item.Substring(0, item.Length - 5).Trim();
                        output += $" {i++}. " + s + Environment.NewLine;
                    }
                    // trim off the training newline
                    output = output.Trim();
                    cleanString = cleanString.Substring(cleanString.IndexOf("</ol>") + 5);
                } else
                {
                    Console.WriteLine("something else");
                    // something else happens, just break out, avoid an infinite loop
                    break;
                }
                output += Environment.NewLine + Environment.NewLine;
            } while (!String.IsNullOrEmpty(cleanString) && !cleanString.StartsWith("<!--more-->"));
                        
            // potentially remove other HTML tags
            return output.Trim();
        }
    }
}