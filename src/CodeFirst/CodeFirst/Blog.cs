using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeFirst.DomainModel
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

    }
}
