using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLOChallenge.Entities
{
    public class Post
    {
        /// <remarks>
        /// The actual data on the server side appears to be an int, but since we want to test invalid data
        /// we're going to leave this as a string and trust that our consumers know what they're doing.
        /// </remarks>
        public string userId { get; set; }
        /// <remarks>
        /// The actual data on the server side appears to be an int, but since we want to test invalid data
        /// we're going to leave this as a string and trust that our consumers know what they're doing.
        /// </remarks>
        public string id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}
