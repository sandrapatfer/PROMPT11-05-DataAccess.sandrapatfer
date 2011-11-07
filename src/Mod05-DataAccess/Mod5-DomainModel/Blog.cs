using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mod5_DomainModel
{
    using Mod05_ChelasDAL.DataAttributes;

    [Table("Blogs")]
    public class Blog
    {
        [PrimaryKey("ID")]
        public int Id { get; set; }
        
        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

//        [Reference("OwnerID")]
//        public User Owner { get; set; }
    }
}
