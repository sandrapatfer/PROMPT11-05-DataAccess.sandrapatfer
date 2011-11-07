namespace Mod5_DomainModel.DomainModelMappers
{
    using System.Data.SqlClient;

    using Mod05_ChelasDAL.Mappers;
    using Mod05_ChelasDAL.Metadata;

    public class BlogMapper : DbAbstractMapper<Blog, int>
    {
        public BlogMapper(SqlConnection connection, SqlTransaction transaction, MetaDataStore metaDataStore, EntityHydrater hydrater, IdentityMap identityMap)
            : base(connection, transaction, metaDataStore, hydrater, identityMap)
        {
        }
    }
}