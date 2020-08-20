using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using OLOChallenge.Entities;

namespace OLOChallenge.Services
{
    public static class JSONPlaceHolder
    {
        public static HttpResponseMessage get_Posts()
        {
            var data = ServiceUtilities.IssueGetRequest("https://jsonplaceholder.typicode.com/posts");
            return data;
        }

        public static HttpResponseMessage post_Posts(Post post)
        {
            var data = ServiceUtilities.IssuePostRequest("https://jsonplaceholder.typicode.com/posts", default);
            var content = new StringContent(
                    JsonConvert.SerializeObject(post),
                    Encoding.UTF8,
                    "application/json");
            return data;
        }
    }
}
