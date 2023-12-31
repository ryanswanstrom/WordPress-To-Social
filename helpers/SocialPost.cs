
using System;
using System.Collections.Generic;

namespace RyanSwanstrom.Function
{
    public class SocialPost
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public string Video { get; set; }        
        public string VideoThumbnail { get; set; }
        public bool IsVideoVertical { get; set; }
        public List<String> Images { get; set; }
        public List<String> Tags { get; set; } = new List<string>();
        public string URL { get; set; }
        public override string ToString()
        {
            return $"{Title}: {URL}";
        }
    }
}