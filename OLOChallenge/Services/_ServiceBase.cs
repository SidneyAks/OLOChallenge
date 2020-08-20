using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace OLOChallenge.Services
{
    public static class HttpClientSingleton
    {
        private static readonly HttpClient client = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
        public static HttpClient Instance => client;
    }

    internal class ServiceUtilities
    {
        public static HttpResponseMessage IssueGetRequest(string url, Dictionary<string, string> additionalHeaders = null)
        {
            var uri = new Uri(url);
            return IssueRequest(uri, null, additionalHeaders, HttpMethod.Get);
        }

        public static HttpResponseMessage IssuePostRequest(string url, HttpContent content, Dictionary<string, string> additionalHeaders = null)
        {
            var uri = new Uri(url);
            return IssueRequest(uri, content, additionalHeaders, HttpMethod.Post);
        }

        private static HttpResponseMessage IssueRequest(Uri url, HttpContent data, Dictionary<string, string> additionalHeaders, HttpMethod method)
        {
            var req = new HttpRequestMessage();
            req.RequestUri = url;

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
            }

            if (method == HttpMethod.Post)
            {
                req.Content = data;
                req.Method = HttpMethod.Post;
            }
            return HttpClientSingleton.Instance.SendAsync(req).Result;
        }
    }
}
