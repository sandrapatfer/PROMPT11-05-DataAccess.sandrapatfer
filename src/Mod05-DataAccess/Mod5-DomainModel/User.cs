namespace Mod5_DomainModel
{
    using Mod05_ChelasDAL.DataAttributes;

    [Table("Users")]
    public class User
    {
        [PrimaryKey("OwnerId")]
        public int OwnerId { get; set; }


        [Column("Name")]
        public string Name { get; set; }
    }
}