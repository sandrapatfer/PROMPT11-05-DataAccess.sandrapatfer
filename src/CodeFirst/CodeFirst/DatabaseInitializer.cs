using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace CodeFirst.DomainModel
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<BlogDbContext>
    {
        #region IDatabaseInitializer<BlogDbContext> Members

        protected override void Seed(BlogDbContext context)
        {
            // set Blog Title unique
            context.Database.ExecuteSqlCommand("alter table blogs add constraint UniqueBlogTitle unique (Title)");

            List<Category> categories = new List<Category>() { 
                new Category() { Name = "News", Description="News"},
                new Category() { Name = "Photos", Description="Photos"} };
            categories.ForEach(c => context.Categories.Add(c));

            Blog b = new Blog() { Title = "My Blog" };
            context.Blogs.Add(b);

            List<Post> posts = new List<Post>() {
                new Post() { Title = "This is new", Blog = b, 
                    Categories = categories.Where(c => c.Name == "News").ToList(), When = DateTime.Now},
                new Post() { Title = "New photo", Blog = b,
                    Categories = categories.Where(c => c.Name == "Photos").ToList(), When = DateTime.Now}
            };
            posts.ForEach(p => context.Posts.Add(p));
        }   

        #endregion
    }
}
