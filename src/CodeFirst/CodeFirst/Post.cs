using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeFirst.DomainModel
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime When { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

    }
}
