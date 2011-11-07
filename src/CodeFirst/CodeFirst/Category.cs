using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CodeFirst.DomainModel
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<Category> ChildCategories { get; set; }

        public int? ParentId { get; set; }
        public Category Parent { get; set; }
    }
}
