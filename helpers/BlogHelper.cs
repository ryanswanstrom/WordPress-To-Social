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
                SocialPost sp = ParseText(p.Content.Rendered, wpclient);
                sp.Title = p.Title.Rendered;
                sp.URL = p.Link;
                socialPosts.Add(sp);
            }

            return socialPosts;
        }

        private static SocialPost ParseText(String text, WordPressClient wpclient)
        {
            Console.WriteLine(text);
            SocialPost sp = new SocialPost();
            String cleanString = text;
            if (String.IsNullOrEmpty(text))
            {
                return sp;
            }
            //int currIx = 0;
            // find paragraphs and extract text
            // continue until <!--more-->
            // is it <p> <ul> or <ol> or <figure>
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
                } 
                else if (cleanString.StartsWith("<figure"))
                {
                    Console.WriteLine("figure");
                    Console.WriteLine(cleanString);

                    //TODO: sp.Video = URL 
                    // pull out the <figure   to </figure> 
                    string fig = cleanString.Substring(0, cleanString.IndexOf("</figure>"));
                    // find the ID after videopress.com/embed/
                    int vidstart = fig.IndexOf("videopress.com/embed/"); //21
                    int vidend = fig.IndexOf('?', vidstart + 21);
                    Console.WriteLine($"start is {vidstart} and the end is {vidend}");
                    string VidID = fig.Substring(vidstart + 21, vidend - vidstart - 21);
                    Console.WriteLine($"Vid ID is: {VidID}");
                    // get videos from Wordpress for last 30 days and find the one with that ID in the SourceUrl
                    var queryBuilder = new MediaQueryBuilder();
                    queryBuilder.MimeType = "video/videopress";
                    queryBuilder.After = DateTime.Now.AddDays(-30);
                    var media = wpclient.Media.QueryAsync(queryBuilder);
                    media.Wait();
                    foreach (MediaItem m in media.Result)
                    {
                        Console.WriteLine(" the id is: " + m.Id);
                        Console.WriteLine("width: " + m.MediaDetails.Width);
                        Console.WriteLine("height: " + m.MediaDetails.Height);
                        Console.WriteLine("source: " + m.SourceUrl); // this is it
                        Console.WriteLine("media: " + m.MediaType);
                        Console.WriteLine("mime: " + m.MimeType);
                        if (m.SourceUrl.Contains(VidID)) {
                            if (m.MediaDetails.Height > m.MediaDetails.Width)
                            {
                                sp.VerticalVideo = m.SourceUrl;
                            } else {
                                sp.HorzVideo = m.SourceUrl;
                            }
                            break;
                        }
                    }
                    // get posterUrl for video cover image sp.VideoCoverImg
                    int posterUrlLoc = fig.IndexOf("posterUrl=");
                    if (posterUrlLoc > -1)
                    { 
                        // get the string and replace %2F with / and %3A with colon
                        string thumb = fig.Substring(posterUrlLoc + 10, fig.IndexOf("&", posterUrlLoc) - posterUrlLoc - 10 );
                        thumb = thumb.Replace("%2F", "/");
                        thumb = thumb.Replace("%3A", ":");
                        if (sp.VerticalVideo != null)
                        {
                            sp.VerticalVideoThumbnail = thumb;
                        } else {
                            sp.HorzVideoThumbnail = thumb;
                        }                       
                        
                        Console.WriteLine($"The thumbnail is: {thumb}");
                    }
                    
                    
                    cleanString = cleanString.Substring(cleanString.IndexOf("</figure>") + 9);
                    
                } 
                else
                {
                    Console.WriteLine("something else");
                    // something else happens, just break out, avoid an infinite loop
                    break;
                }
                if ( !String.IsNullOrEmpty(output) )
                {
                    output += Environment.NewLine + Environment.NewLine;
                }
            } while (!String.IsNullOrEmpty(cleanString) && !cleanString.StartsWith("<!--more-->"));
                        
            
            sp.Text = output;
            return sp;
        }
    }
}