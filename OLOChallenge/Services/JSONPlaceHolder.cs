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
        public static HttpResponseMessage get_Posts(string argument = null)
        {
            var url =
                argument == null ?
                "https://jsonplaceholder.typicode.com/posts" :
                "https://jsonplaceholder.typicode.com/posts" + $"/{argument}";
            var data = ServiceUtilities.IssueGetRequest(url);
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
