using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OLOChallenge.Services;

namespace OLOChallenge.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void basicTest()
        {
            var foo = JSONPlaceHolder.get_Posts();
            var bar = JSONPlaceHolder.post_Posts(
                new Entities.Post() { 
                    title = "Hello World",
                    body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    userId = 1
                });
        }
    }
}
