using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Mod05_ChelasDAL.Metadata;
using Mod5_DomainModel.DomainModelMappers;
using Mod05_ChelasDAL.Mappers;

namespace Mod5_DomainModel.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DomainModelTestDB
    {
        public DomainModelTestDB()
        {
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


        BlogMapper _blogMapper1;
        BlogMapper _blogMapper2;
        SqlTransaction _currentTransaction1;
        SqlTransaction _currentTransaction2;

        [TestInitialize()]
        public void TestInitializer()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @".\SQLEXPRESS";
            builder.IntegratedSecurity = true;
            builder.InitialCatalog = "DBBlogs";
            SqlConnection connection1 = new SqlConnection(builder.ConnectionString);
            connection1.Open();
            _currentTransaction1 = connection1.BeginTransaction();
            SqlConnection connection2 = new SqlConnection(builder.ConnectionString);
            connection2.Open();
            _currentTransaction2 = connection2.BeginTransaction();

            MetaDataStore metaDataStore = new MetaDataStore();
            metaDataStore.BuildTableInfoFor<Blog>();

            var identityMap1 = new IdentityMap();
            var identityMap2 = new IdentityMap();

            _blogMapper1 = new BlogMapper(connection1,
                _currentTransaction1,
                metaDataStore,
                new EntityHydrater(metaDataStore, identityMap1),
                identityMap1);
            _blogMapper2 = new BlogMapper(connection2,
                _currentTransaction2,
                metaDataStore,
                new EntityHydrater(metaDataStore, identityMap2),
                identityMap2);
        }

        [TestMethod]
        public void TestInsertAndGetBlog()
        {
            Blog blog = new Blog() { Name = "teste db1", Description = "teste um" };
            Blog blogInserted = _blogMapper1.Insert(blog);
            _currentTransaction1.Commit();
            Blog blogRetrieved = _blogMapper2.GetById(blogInserted.Id);
            Assert.AreEqual(blogInserted.Name, blogRetrieved.Name);
        }

        [TestMethod]
        public void TestInsertAndUpdate()
        {
            Blog blog = new Blog() { Name = "teste db2", Description = "teste dois" };
            _blogMapper1.Insert(blog);
            _currentTransaction1.Commit();
            blog.Name = "teste db 2";
            _blogMapper2.Update(blog);
            _currentTransaction2.Commit();
            Blog blogUpdated = _blogMapper1.GetById(blog.Id);
            Assert.AreEqual(blog.Name, blogUpdated.Name);
        }

        [TestMethod]
        public void TestInsertAndDelete()
        {
            Blog blog = new Blog() { Name = "teste db3", Description = "teste tres" };
            _blogMapper1.Insert(blog);
            _currentTransaction1.Commit();
            _blogMapper2.Delete(blog);
            _currentTransaction2.Commit();

            Blog blogDeleted = _blogMapper1.GetById(blog.Id); // identity map 1 still has the object
            Assert.AreEqual(blog.Name, blogDeleted.Name);
        }
    }
}