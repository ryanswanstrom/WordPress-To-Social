
using System;
using System.Collections.Generic;

namespace RyanSwanstrom.Function
{
    public class SocialPost
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public string VerticalVideo { get; set; }        
        public string VerticalVideoThumbnail { get; set; }
        public string HorzVideo { get; set; }        
        public string HorzVideoThumbnail { get; set; }
        public List<String> Images { get; set; }
        public string URL { get; set; }
        public override string ToString()
        {
            return $"{Title}: {URL}";
        }
    }
}