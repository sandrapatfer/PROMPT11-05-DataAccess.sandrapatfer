using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeFirst.DomainModel
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime When { get; set; }
        public bool Approved { get; set; }
        public string Author { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
