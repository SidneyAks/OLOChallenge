using Microsoft.VisualStudio.TestTools.UnitTesting;
using OLOChallenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using OLOChallenge.Helpers;
using OLOChallenge.Authentication;
using OLOChallenge.Entities;

namespace OLOChallenge.Tests
{
    /// <summary>
    /// Tests the post method on the post route
    /// </summary>
    /// 
    /// <remarks>
    /// The post operation on the posts route is poorly defined. This appears to be a basic way of teaching how to talk
    /// to a web service with very little in the way of input validation, it makes it hard to write a tests for it, as
    /// there isn't much documentation on how it _should_ behave. For the purposes of this exercise, I'm going to have
    /// to assume some of the business rules.
    /// 
    /// 1.) The route is plural. I would expect since it's plural it can take one or more post objects (in fact, it appears
    /// to accept multiple post objects, but the results don't appear to indicate multiple were saved).
    /// 
    /// 2.) The route accepts a defined object but in reality doesn't appear to validate it at all, it simply returns a 201
    /// even if the data it was passed makes no sense. I would expect there to be some validation (At the very list that 
    /// the title and body are valid, and that the user id exists!). I'm going to write tests as such, but they will fail.
    /// If I were in a feedback loop with developers I would hope these requirements were either laid out prior to code authorship 
    /// (although honestly I'd expect that if a user were trying to create a post and the title and body were both null, throwing
    /// an error wouldn't need to be an explicit requirement)
    /// 
    /// 3.) The post object contains a userId field -- I expect the create or update system would either 1.) Throw an error if the 
    /// user tries to create a post with a different ID than their own or 2.) Ignore the userID field and create the post with the id
    /// of the authenticated user (this seems less preferable to me, but I would defer to the product owner) . Of course since we 
    /// don't actually authenticate into this system it's impossible to test either of the scenarios.
    /// </remarks>
    [TestClass]
    public class post_Posts
    {
        [TestMethod]
        public void WithValidPostIs200AndReturnsPostsWithUpdateFields()
        {
            var session = Auth.Authenticate(4);
            var inputdata = new Post()
            {
                title = "Hello World",
                body = Lorem.Text,
                userId = session.UserID
            };

            var data = session.JSONPlaceHolder_post_Posts(inputdata);
            Assert.AreEqual(HttpStatusCode.Created, data.StatusCode, "Unexpected status code when creating post");

            var createdPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(Post)),
                "Unable to parse response into Post");

            Assert.AreEqual(inputdata.title, createdPost.title, "Post returned from creation is incorrect");
            Assert.AreEqual(inputdata.body, createdPost.body, "Post returned from creation is incorrect");
            Assert.AreEqual(inputdata.userId, createdPost.userId, "Post returned from creation is incorrect");

            var retrievedPostResponse = session.JSONPlaceHolder_get_Posts(createdPost.id);
            Assert.AreEqual(HttpStatusCode.OK, retrievedPostResponse.StatusCode, "Unexpected response code when retrieving created post");

            var retrievedPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(retrievedPostResponse.Content.ReadAsStringAsync().Result, typeof(Post)),
                "Unable to parse response into Post");

            Assert.AreEqual(inputdata.title, retrievedPost.title, "Post saved after creation is incorrect");
            Assert.AreEqual(inputdata.body, retrievedPost.body, "Post saved after creation is incorrect");
            Assert.AreEqual(inputdata.userId, retrievedPost.userId, "Post saved after creation is incorrect");
        }

        [TestMethod]
        public void WithInvalidJsonReturns400Error()
        {
            var session = Auth.Authenticate(4);
            var entity = new DirectStringContent() {
                    Content = "{\"userId\":\"4\",\"id\":null,\"titl"/*e\":\"Hello World\",\"body\":\"Lorem\"}"*/,
                    Encoding = Encoding.UTF8,
                    MimeType = "application/json"
                };
            var data = session.JSONPlaceHolder_post_Posts(entity);
            Assert.AreEqual(HttpStatusCode.BadRequest, data.StatusCode);
        }

        [TestMethod]
        public void FuzzWithValidPostIs200AndReturnsPostsWithUpdateFields()
        {
            var session = Auth.Authenticate(4);

            //Ideally we would like to run a loop here with data generated from a commercial or open source
            //fuzz generator, but since this is a coding example I'll just make sure a few special cases
            //are not allowed.
            var FuzzyData = new List<string>() { "🙃", "0xBAADF00D", "eval('while(true) {}')" };

            foreach(var fuzz in FuzzyData)
            {
                var inputdata = new Post()
                {
                    title = fuzz,
                    body = fuzz,
                    userId = session.UserID
                };

                var data = session.JSONPlaceHolder_post_Posts(inputdata);
                Assert.AreEqual(HttpStatusCode.Created, data.StatusCode, "Unexpected status code when creating post");

                var createdPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(Post)),
                    "Unable to parse response into Post");

                Assert.AreEqual(inputdata.title, createdPost.title, "Post returned from creation is incorrect");
                Assert.AreEqual(inputdata.body, createdPost.body, "Post returned from creation is incorrect");
                Assert.AreEqual(inputdata.userId, createdPost.userId, "Post returned from creation is incorrect");

                var retrievedPostResponse = session.JSONPlaceHolder_get_Posts(createdPost.id);
                Assert.AreEqual(HttpStatusCode.OK, retrievedPostResponse.StatusCode, "Unexpected response code when retrieving created post");

                var retrievedPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(retrievedPostResponse.Content.ReadAsStringAsync().Result, typeof(Post)),
                    "Unable to parse response into Post");

                Assert.AreEqual(inputdata.title, retrievedPost.title, "Post saved after creation is incorrect");
                Assert.AreEqual(inputdata.body, retrievedPost.body, "Post saved after creation is incorrect");
                Assert.AreEqual(inputdata.userId, retrievedPost.userId, "Post saved after creation is incorrect");
            }
        }

        [TestMethod]
        public void WithoutSessionIs401Error()
        {
            var session = SessionObject.UnauthenticatedSession;
            var data = session.JSONPlaceHolder_post_Posts(
            new Post()
            {
                title = "Hello World",
                body = Lorem.Text,
                userId = "1"
            });
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithInvalidSessionIs401Error()
        {
            var session = SessionObject.InvalidSession;
            var data = session.JSONPlaceHolder_post_Posts(
            new Post()
            {
                title = "Hello World",
                body = Lorem.Text,
                userId = "1"
            });
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithNullTitleIs400Error()
        {
            var session = Auth.Authenticate(4);
            var data = session.JSONPlaceHolder_post_Posts(
                new Post()
                {
                    title = null,
                    body = Lorem.Text,
                    userId = session.UserID
                });
            Assert.AreEqual(HttpStatusCode.BadRequest, data.StatusCode);
        }

        [TestMethod]
        public void WithNullBodyIs400Error()
        {
            var session = Auth.Authenticate(4);
            var data = session.JSONPlaceHolder_post_Posts(
                new Post()
                {
                    title = "Hello World",
                    body = null,
                    userId = session.UserID
                });
            Assert.AreEqual(HttpStatusCode.BadRequest, data.StatusCode);
        }

        [TestMethod]
        public void WithIncorrectUserIDIs401Error()
        {
            var session = Auth.Authenticate(4);
            var data = session.JSONPlaceHolder_post_Posts(
                new Post()
                {
                    title = "Hello World",
                    body = Lorem.Text,
                    userId = "1"
                });
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }
    }
}
