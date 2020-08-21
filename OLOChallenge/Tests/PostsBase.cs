using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OLOChallenge.Authentication;
using OLOChallenge.Entities;
using OLOChallenge.Helpers;
using OLOChallenge.Services;
using System.Net;

namespace OLOChallenge.Tests
{
    [TestClass]
    public class PostsBase
    {
        protected static void CreateGenericPostForPrecondition(SessionObject session, out Post CreatedPost)
        {
            CreatedPost = null;

            var inputdata = new Post()
            {
                title = "Hello World",
                body = Lorem.Text,
                userId = session.UserID
            };

            var data = session.JSONPlaceHolder_post_Posts(inputdata);

            CreatedPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(Post)),
                "Unable to parse response into Post");

            var RetrievedPost = session.JSONPlaceHolder_get_Posts(CreatedPost.id);
            Assert.AreEqual(RetrievedPost.StatusCode, HttpStatusCode.OK);
            Log.WriteInfo($"Created generic Post object, required as test precondition");
        }
    }
}
