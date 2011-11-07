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
    public class DomainModelTest
    {
        public DomainModelTest()
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


        BlogMapper _blogMapper;
        SqlTransaction _currentTransaction;
        bool _commit = false;

        [TestInitialize()]
        public void TestInitializer()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @".\SQLEXPRESS";
            builder.IntegratedSecurity = true;
            builder.InitialCatalog = "DBBlogs";
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
            _currentTransaction = connection.BeginTransaction();

            MetaDataStore metaDataStore = new MetaDataStore();
            metaDataStore.BuildTableInfoFor<Blog>();

            var identityMap = new IdentityMap();

            _blogMapper = new BlogMapper(connection,
                _currentTransaction,
                metaDataStore,
                new EntityHydrater(metaDataStore, identityMap),
                identityMap);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (_commit)
            {
                _currentTransaction.Commit();
            }
            else
            {
                _currentTransaction.Rollback();
            }
        }

        [TestMethod]
        public void TestInsertBlog()
        {
            Blog blog = new Blog() { Name="teste1", Description="teste um"};
            Blog blogInserted = _blogMapper.Insert(blog);
            Assert.AreEqual(blog.Name, blogInserted.Name);

            // not closing the transaction, so the actions are all rolled back
        }

        [TestMethod]
        public void TestInsertAndGetBlog()
        {
            Blog blog = new Blog() { Name = "teste1", Description = "teste um" };
            Blog blogInserted = _blogMapper.Insert(blog);
            Blog blogRetrieved = _blogMapper.GetById(blogInserted.Id);
            Assert.AreEqual(blogInserted.Name, blogRetrieved.Name);
        }

        [TestMethod]
        public void TestInsertAndUpdate()
        {
            Blog blog = new Blog() { Name = "teste1", Description = "teste um" };
            _blogMapper.Insert(blog);
            blog.Name = "teste2";
            _blogMapper.Update(blog);
            Blog blogUpdated = _blogMapper.GetById(blog.Id);
            Assert.AreEqual(blog.Name, blogUpdated.Name);
        }

    }
}