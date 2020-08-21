using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using OLOChallenge.Authentication;
using OLOChallenge.Entities;

namespace OLOChallenge.Services
{
    public static class JSONPlaceHolder
    {
        public static HttpResponseMessage JSONPlaceHolder_get_Posts(this SessionObject session, string argument = null)
        {
            var url =
                argument == null ?
                "/posts" :
                "/posts" + $"/{argument}";
            var data = session.IssueGetRequest(url);
            return data;
        }

        public static HttpResponseMessage JSONPlaceHolder_post_Posts(this SessionObject session, IEntity post)
        {
            var data = session.IssuePostRequest("/posts", post.GetStringContent());
            return data;
        }

        public static HttpResponseMessage JSONPlaceHolder_put_Posts(this SessionObject session, string PostID, IEntity post)
        {
            var data = session.IssuePutRequest("/posts" + $"/{PostID}", post.GetStringContent());
            return data;
        }

        public static HttpResponseMessage JSONPlaceHolder_delete_Posts(this SessionObject session, string PostID)
        {
            var data = session.IssueDeleteRequest("/posts" + $"/{PostID}");
            return data;
        }
    }
}
