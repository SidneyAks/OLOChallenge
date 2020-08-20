using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OLOChallenge.Entities;
using OLOChallenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLOChallenge.Tests
{
    [TestClass]
    public class get_Posts
    {
        [TestMethod]
        public void EnsureGetPostWithoutArguementIs200AndReturnsListOfPosts()
        {
            var data = JSONPlaceHolder.get_Posts();
            Assert.AreEqual(System.Net.HttpStatusCode.OK, data.StatusCode);
            var posts = JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(List<Post>));
        }

        [TestMethod]
        public void EnsureGetPostWithNumericArgIs200AndReturnsListOfPosts()
        {
            foreach (var i in Enumerable.Range(1, 100))
            {
                var data = JSONPlaceHolder.get_Posts(i.ToString());
                Assert.AreEqual(System.Net.HttpStatusCode.OK, data.StatusCode);
                var posts = JsonConvert.DeserializeObject(data.Content.ReadAsStringAsync().Result, typeof(Post));
            }
        }

        [TestMethod]
        public void EnsureGetPostWithNumericArgOutOfBoundsIsNotFound()
        {
            var data = JSONPlaceHolder.get_Posts("0");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, data.StatusCode);

            data = JSONPlaceHolder.get_Posts(Int32.MaxValue.ToString());
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, data.StatusCode);
        }

        [TestMethod]
        public void EnsureGetPostWithNonNumericArgIsError()
        {
            var data = JSONPlaceHolder.get_Posts("Hello World");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, data.StatusCode);
        }

        [TestMethod]
        public void EnsureGetPostWithInjectionArgIsError()
        {
            //Ideally we would like to run a loop here with a commercial or open source injection tester,
            //but since this is a coding example I'll just make sure "' or 1=1" is not allowed
            var data = JSONPlaceHolder.get_Posts("1' or '1'='1' /*");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, data.StatusCode);
        }

    }
}
