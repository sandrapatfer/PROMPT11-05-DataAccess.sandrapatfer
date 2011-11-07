using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data;

namespace CodeFirst.DomainModel.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CodeFirstTest
    {
        public CodeFirstTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestCreateDatabaseScript()
        {
            var context = new BlogDbContext();
            IObjectContextAdapter iObject = (IObjectContextAdapter)context;
            string script = iObject.ObjectContext.CreateDatabaseScript();
            Assert.IsNotNull(script);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Data.Entity.Infrastructure.DbUpdateException))]
        public void TestIsBlogTitleUnique()
        {
            Database.SetInitializer(new DatabaseInitializer());
            var context = new BlogDbContext();

            context.Blogs.Add(new Blog() { Title = "abc" });
            context.SaveChanges();

            context.Blogs.Add(new Blog() { Title = "abc" });
            context.SaveChanges();
        }
    }
}
