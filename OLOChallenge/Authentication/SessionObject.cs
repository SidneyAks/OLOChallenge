using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace OLOChallenge.Authentication
{
    /// <summary>
    /// A session object returned via the authentication call
    /// </summary>
    /// 
    /// <remarks>
    /// A real object to handle a session might have a bearer token or a session guid. Possibly some metadata about which 
    /// environment (dev, integration, stage, prod) it was talking to.
    /// </remarks>

    public class SessionObject
    {
        public static SessionObject UnauthenticatedSession => new SessionObject() { UserID = null };
        public static SessionObject InvalidSession => new SessionObject() { UserID = "0xBAADF00D" };
        public static SessionObject AdminSession => new SessionObject() { UserID = "ADMIN" };

        public string UserID;
        public string EnvironmentHostName => "https://jsonplaceholder.typicode.com";

        public HttpResponseMessage IssueGetRequest(string endpoint, Dictionary<string, string> additionalHeaders = null)
        {
            return IssueRequest(endpoint, null, additionalHeaders, HttpMethod.Get);
        }

        public HttpResponseMessage IssuePostRequest(string endpoint, HttpContent content, Dictionary<string, string> additionalHeaders = null)
        {
            return IssueRequest(endpoint, content, additionalHeaders, HttpMethod.Post);
        }

        public HttpResponseMessage IssuePutRequest(string endpoint, HttpContent content, Dictionary<string, string> additionalHeaders = null)
        {
            return IssueRequest(endpoint, content, additionalHeaders, HttpMethod.Put);
        }

        public HttpResponseMessage IssueDeleteRequest(string endpoint, Dictionary<string,string> additionalHeaders = null)
        {
            return IssueRequest(endpoint, null, additionalHeaders, HttpMethod.Delete);
        }

        private HttpResponseMessage IssueRequest(string url, HttpContent data, Dictionary<string, string> additionalHeaders, HttpMethod method)
        {
            var req = new HttpRequestMessage();
            req.RequestUri = new Uri($"{EnvironmentHostName}/{url.TrimStart('/')}");

            //Here I would place the "session ID" request header if we were working with a real API.
            req.Headers.Add("SESSION_ID", UserID);

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
            }

            req.Method = method;
            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                req.Content = data;
            }
            return HttpClientSingleton.Instance.SendAsync(req).Result;
        }

        public static class HttpClientSingleton
        {
            private static readonly HttpClient client = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            public static HttpClient Instance => client;
        }
    }
}
