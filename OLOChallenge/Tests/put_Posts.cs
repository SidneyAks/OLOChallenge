using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OLOChallenge.Authentication;
using OLOChallenge.Entities;
using OLOChallenge.Helpers;
using OLOChallenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OLOChallenge.Tests
{
    /// <summary>
    /// Tests the post method on the post route.
    /// </summary>
    /// 
    /// <remarks>
    /// This API is a hair more straightforeward than the those of the post verb. Since the url takes a single parameter (the post id) and 
    /// must have it, it appears only one post can be modified at a time (which makes sense). Given that restraint it should be a bit easier
    /// to test, however we do have some basic things we still need to check for, such as if the session ID is checked and if the updated
    /// post object contains valid data.
    /// </remarks>
    [TestClass]
    public class put_Posts
    {
        private static void CreateGenericPostForPrecondition(SessionObject session, out Post CreatedPost)
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

            Assert.IsNotNull(CreatedPost, "Generic Post for test Is Null");
        }

        [TestMethod]
        public void WithExistingPostAndValidDataIs200AndReturnsPostWithUpdatedFields()
        {
            var session = Auth.Authenticate(4);
            Post CreatedPost = null;

            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to modify with put verb");

            CreatedPost.title += "Modified";
            CreatedPost.body += "Modified";

            var data = session.JSONPlaceHolder_put_Posts(CreatedPost.id,CreatedPost);
            Assert.AreEqual(HttpStatusCode.OK, data.StatusCode, "Unexpected status code when modifying post");

            var updatedPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(Post)),
                "Unable to parse response into Post");

            Assert.AreEqual(CreatedPost.title, updatedPost.title, "Post returned from modification is incorrect");
            Assert.AreEqual(CreatedPost.body, updatedPost.body, "Post returned from modification is incorrect");
            Assert.AreEqual(CreatedPost.userId, updatedPost.userId, "Post returned from modification is incorrect");

            var retrievedPostResponse = session.JSONPlaceHolder_get_Posts(updatedPost.id);
            Assert.AreEqual(HttpStatusCode.OK, retrievedPostResponse.StatusCode, "Unexpected response code when retrieving modification post");

            var retrievedPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(retrievedPostResponse.Content.ReadAsStringAsync().Result, typeof(Post)),
                "Unable to parse response into Post");

            Assert.AreEqual(CreatedPost.title, retrievedPost.title, "Post saved after modification is incorrect");
            Assert.AreEqual(CreatedPost.body, retrievedPost.body, "Post saved after modification is incorrect");
            Assert.AreEqual(CreatedPost.userId, retrievedPost.userId, "Post saved after modification is incorrect");
        }

        [TestMethod]
        public void FuzzWithExistingPostAndValidDataIs200AndReturnsPostWithUpdatedFields()
        {
            var session = Auth.Authenticate(4);
            Post CreatedPost = null;

            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to modify with put verb");

            //Ideally we would like to run a loop here with data generated from a commercial or open source
            //fuzz generator, but since this is a coding example I'll just make sure a few special cases
            //are not allowed.
            var FuzzyData = new List<string>() { "🙃", "0xBAADF00D", "eval('while(true) {}')" };

            foreach (var fuzz in FuzzyData)
            {
                CreatedPost.title = fuzz;
                CreatedPost.body = fuzz;

                var data = session.JSONPlaceHolder_put_Posts(CreatedPost.id, CreatedPost);
                Assert.AreEqual(HttpStatusCode.OK, data.StatusCode, "Unexpected status code when modifying post");

                var updatedPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(Post)),
                    "Unable to parse response into Post");

                Assert.AreEqual(CreatedPost.title, updatedPost.title, "Post returned from modification is incorrect");
                Assert.AreEqual(CreatedPost.body, updatedPost.body, "Post returned from modification is incorrect");
                Assert.AreEqual(CreatedPost.userId, updatedPost.userId, "Post returned from modification is incorrect");

                var retrievedPostResponse = session.JSONPlaceHolder_get_Posts(updatedPost.id);
                Assert.AreEqual(HttpStatusCode.OK, retrievedPostResponse.StatusCode, "Unexpected response code when retrieving modification post");

                var retrievedPost = ExtraAssert.Succeeds(() => (Post)JsonConvert.DeserializeObject(retrievedPostResponse.Content.ReadAsStringAsync().Result, typeof(Post)),
                    "Unable to parse response into Post");

                Assert.AreEqual(CreatedPost.title, retrievedPost.title, "Post saved after modification is incorrect");
                Assert.AreEqual(CreatedPost.body, retrievedPost.body, "Post saved after modification is incorrect");
                Assert.AreEqual(CreatedPost.userId, retrievedPost.userId, "Post saved after modification is incorrect");
            }
        }

        [TestMethod]
        public void WithoutSessionis401Error()
        {
            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(Auth.Authenticate(4), out CreatedPost), "Unable to create post to modify with put verb");

            var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_put_Posts(CreatedPost.id, CreatedPost);
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithInvalidSessionIs401Error()
        {
            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(Auth.Authenticate(4), out CreatedPost), "Unable to create post to modify with put verb");

            var data = SessionObject.InvalidSession.JSONPlaceHolder_put_Posts(CreatedPost.id, CreatedPost);
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithNullTitleIs400Error()
        {
            var session = Auth.Authenticate(4);

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to modify with put verb");

            CreatedPost.title = null;
            var data = SessionObject.InvalidSession.JSONPlaceHolder_put_Posts(CreatedPost.id, CreatedPost);

            Assert.AreEqual(HttpStatusCode.BadRequest, data.StatusCode);
        }

        [TestMethod]
        public void WithNullBodyIs400Error()
        {
            var session = Auth.Authenticate(4);

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to modify with put verb");

            CreatedPost.body = null;
            var data = SessionObject.InvalidSession.JSONPlaceHolder_put_Posts(CreatedPost.id, CreatedPost);

            Assert.AreEqual(HttpStatusCode.BadRequest, data.StatusCode);
        }

        [TestMethod]
        public void WithIncorrectUserIDIs401Error()
        {
            var session = Auth.Authenticate(4);

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to modify with put verb");

            CreatedPost.userId = "1";
            var data = SessionObject.InvalidSession.JSONPlaceHolder_put_Posts(CreatedPost.id, CreatedPost);
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithNonExistentPostIdIs404Error()
        {
            var session = Auth.Authenticate(4);

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to modify with put verb");

            //Well assume that there are not this many posts in the database.
            CreatedPost.id = Int64.MaxValue.ToString();
            var data = SessionObject.InvalidSession.JSONPlaceHolder_put_Posts(CreatedPost.id, CreatedPost);
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
        }

        [TestMethod]
        public void WithInconsistentIDBetweenUrlandBodyIs400Error()
        {
            var session = Auth.Authenticate(4);

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to modify with put verb");

            //Well assume that there are not this many posts in the database.
            var data = SessionObject.InvalidSession.JSONPlaceHolder_put_Posts("1", CreatedPost);
            Assert.AreEqual(HttpStatusCode.BadRequest, data.StatusCode);
        }

    }
}
