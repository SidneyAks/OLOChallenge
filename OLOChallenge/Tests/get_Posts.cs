using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OLOChallenge.Entities;
using OLOChallenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using OLOChallenge.Authentication;
using OLOChallenge.Helpers;

namespace OLOChallenge.Tests
{
    /// <summary>
    /// Tests the get method on the post route.
    /// </summary>
    /// 
    /// <remarks>
    /// This endpoint is fairly obvious, it can return one or more posts depending on the arguments. I would expect
    /// that it should either return a list of posts, or when provided with an argument it should return the post(s?)
    /// specified by the arguement. It appears that initially it can return only one post, however since the route
    /// name is plural, I would push to accept an array of post id's to return, or change the route name to the singular form.
    /// 
    /// As this endpoint appears to be a READ operation, I would not expect it to do any updating logic that I would need to test.
    /// I would expect basic tests to include positive scenario tests, tests for when a post ID is not found, tests for when input
    /// is invalid, and tests for security related areas such as sql injection, finally it should include a fuzz test to see
    /// if any invalid input causes strange api results that could be used to snoop for more dangerous exploits.
    /// 
    /// Finally I would expect that this type of operation is not session guarded, as such we should be able to call it
    /// with an unauthenticatedsession, or a valid session. Finally, I would expect if it were called with an invalid session
    /// One of two things would happen. Either 1.) The data would be returned without error, or 2.) a bad request would result.
    /// I would expect the first result, as an endpoint that is not session guarded would not need to spend the compute time
    /// to validate that the session is actuall valid.
    /// </remarks>
    [TestClass]
    public class get_Posts
    {
        [TestMethod]
        public void WithoutArguementWithoutSessionIs200AndReturnsListOfPosts()
        {
            var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_get_Posts();
            Assert.AreEqual(HttpStatusCode.OK, data.StatusCode);
            var posts = ExtraAssert.Succeeds(() => JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(List<Post>)),
                "Unable to parse response into List of Posts");
        }

        [TestMethod]
        public void WithoutArguementWithSessionIs200AndReturnsListOfPosts()
        {
            var data = Auth.Authenticate(4).JSONPlaceHolder_get_Posts();
            Assert.AreEqual(HttpStatusCode.OK, data.StatusCode);
            var posts = ExtraAssert.Succeeds(() => JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(List<Post>)),
                "Unable to parse response into List of Posts");
        }

        [TestMethod]
        public void WithoutArguementWithInvalidSessionIs200AndReturnsListOfPosts()
        {
            var data = SessionObject.InvalidSession.JSONPlaceHolder_get_Posts();
            Assert.AreEqual(HttpStatusCode.OK, data.StatusCode);
            var posts = ExtraAssert.Succeeds(() => JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(List<Post>)),
                "Unable to parse response into List of Posts");
        }

        [TestMethod]
        public void WithNumericArgIs200AndReturnsPosts()
        {
            foreach (var i in Enumerable.Range(1, 100))
            {
                var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_get_Posts(i.ToString());
                Assert.AreEqual(HttpStatusCode.OK, data.StatusCode);
                var posts = ExtraAssert.Succeeds(() => JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(Post)),
                "Unable to parse response into Post");
            }
        }

        [TestMethod]
        public void WithNonExistentPostIDIs404Error()
        {
            var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_get_Posts("0");
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);

            data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_get_Posts(Int64.MaxValue.ToString());
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
        }

        [TestMethod]
        public void WithNonNumericArgIs404Error()
        {
            var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_get_Posts("Hello World");
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
        }

        [TestMethod]
        public void WithInjectionArgIs404Error()
        {
            //Ideally we would like to run a loop here with a commercial or open source injection tester,
            //but since this is a coding example I'll just make sure "' or 1=1" is not allowed
            var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_get_Posts("1' or '1'='1' /*");
            Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
        }

        [TestMethod]
        public void WithFuzzyArgIs404Error()
        {
            //Ideally we would like to run a loop here with data generated from a commercial or open source
            //fuzz generator, but since this is a coding example I'll just make sure a few special cases
            //are not allowed.
            var FuzzyData = new List<string>() { "🙃", "0xBAADF00D", "eval('while(true) {}')" };

            foreach (var fuzz in FuzzyData)
            {
                var data = SessionObject.UnauthenticatedSession.JSONPlaceHolder_get_Posts(fuzz);
                Assert.AreEqual(HttpStatusCode.NotFound, data.StatusCode);
            }
        }

    }
}
