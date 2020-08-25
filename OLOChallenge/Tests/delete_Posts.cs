using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    [TestClass]
    public class delete_Posts : PostsBase
    {
        [TestMethod]
        public void WithExistingPostIs200AndPostIsDeleted()
        {
            var session = Auth.Authenticate(4);

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session, out CreatedPost), "Unable to create post to delete");

            var data = session.JSONPlaceHolder_delete_Posts(CreatedPost.id);
            Log.WriteInfo($"Issued Request to delete post with ID {CreatedPost.id}, Response is {(int)data.StatusCode} {data.StatusCode}");
            Assert.AreEqual(HttpStatusCode.OK, data.StatusCode);

            var deletedPostRequest = session.JSONPlaceHolder_get_Posts(CreatedPost.id);
            Log.WriteInfo($"Issued Request to retrieve post with ID {CreatedPost.id}, Response is {(int)deletedPostRequest.StatusCode} {deletedPostRequest.StatusCode}");
            Assert.AreEqual(HttpStatusCode.NotFound, deletedPostRequest.StatusCode);
        }

        [TestMethod]
        public void WithNonExistentPostIdIs404Error()
        {
            var session = Auth.Authenticate(4);

            //Well assume that there are not this many posts in the database.
            var data = session.JSONPlaceHolder_delete_Posts(Int64.MaxValue.ToString());
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
        }

        public void WithNonNumericArgIs404Error()
        {
            var session = Auth.Authenticate(4);

            var data = session.JSONPlaceHolder_delete_Posts("Hello World");
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
        }
        public void WithInjectionArgIs404Error()
        {
            var session = Auth.Authenticate(4);

            //Ideally we would like to run a loop here with a commercial or open source injection tester,
            //but since this is a coding example I'll just make sure "' or 1=1" is not allowed
            var data = session.JSONPlaceHolder_delete_Posts("1' or '1'='1' /*");
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
        }

        [TestMethod]
        public void WithFuzzyArgIs404Error()
        {
            var session = Auth.Authenticate(4);

            //Ideally we would like to run a loop here with data generated from a commercial or open source
            //fuzz generator, but since this is a coding example I'll just make sure a few special cases
            //are not allowed.
            var FuzzyData = new List<string>() { "🙃", "0xBAADF00D", "eval('while(true) {}')" };

            foreach (var fuzz in FuzzyData)
            {
                var data = session.JSONPlaceHolder_delete_Posts(fuzz);
                Log.WriteInfo($"Issued Request to delete post with ID {fuzz}, Response is  {(int)data.StatusCode} {data.StatusCode}");
                Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
            }
        }

        [TestMethod]
        public void WithOutSessionIs401Error()
        {
            //Data doesn't matter, we shouldn't get that far
            var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_delete_Posts(Int64.MaxValue.ToString());
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithInvalidSessionIs401Error()
        {
            //Data doesn't matter, we shouldn't get that far
            var data = SessionObject.InvalidSession.JSONPlaceHolder_delete_Posts(Int64.MaxValue.ToString());
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithOtherUsersPostIDAsNonAdminIs401Error()
        {
            var session1 = Auth.Authenticate(4);
            var session2 = Auth.Authenticate(5);

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session1, out CreatedPost), "Unable to create post to delete");

            var data = session2.JSONPlaceHolder_delete_Posts(CreatedPost.id);
            Assert.AreEqual(HttpStatusCode.Unauthorized, data.StatusCode);
        }

        [TestMethod]
        public void WithOtherUsersPostIDAsAdminIs200OK()
        {
            var session1 = Auth.Authenticate(4);
            var session2 = SessionObject.AdminSession;

            Post CreatedPost = null;
            ExtraAssert.PreconditionSucceeds(() => CreateGenericPostForPrecondition(session1, out CreatedPost), "Unable to create post to delete");

            var data = session2.JSONPlaceHolder_delete_Posts(CreatedPost.id);
            Assert.AreEqual(HttpStatusCode.OK, data.StatusCode);
        }

    }
}
