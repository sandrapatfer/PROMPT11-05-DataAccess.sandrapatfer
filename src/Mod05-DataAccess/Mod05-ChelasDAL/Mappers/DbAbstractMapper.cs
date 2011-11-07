namespace Mod05_ChelasDAL.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using Mod05_ChelasDAL.Helpers;
    using Mod05_ChelasDAL.Metadata;

    public class DbAbstractMapper<TEntity, TKey> : IEntityMapper<TEntity, TKey> where TEntity : new()
    {
        private readonly SqlConnection connection;
        private readonly SqlTransaction transaction;
        protected MetaDataStore MetaDataStore { get; private set; }
        protected EntityHydrater Hydrater { get; private set; }
        protected IdentityMap IdentityMap { get; private set; }

        static DbAbstractMapper()
        {
        }

        protected DbAbstractMapper(SqlConnection connection, SqlTransaction transaction, MetaDataStore metaDataStore,
                                 EntityHydrater hydrater, IdentityMap identityMap)
        {
            this.connection = connection;
            this.transaction = transaction;
            MetaDataStore = metaDataStore;
            Hydrater = hydrater;
            this.IdentityMap = identityMap;
        }

        protected SqlCommand CreateCommand()
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            return command;
        }


        #region Implementation of IEntityMapper<TEntity,TKey>

        public IQueryable<TEntity> FindAll()
        {
            using (var command = CreateCommand())
            {
                var tableInfo = MetaDataStore.GetTableInfoFor<TEntity>();

                command.CommandText = tableInfo.GetSelectStatementForAllFields().ToString();
                return Hydrater.HydrateEntities<TEntity>(command).AsQueryable();
            }
        }

        public TEntity GetById(TKey id)
        {
            var cachedEntity = this.IdentityMap.TryToFind(typeof(TEntity), id);
            if (cachedEntity != null) {
                return (TEntity)cachedEntity;
            }

            using (var command = CreateCommand())
            {
                var tableInfo = MetaDataStore.GetTableInfoFor<TEntity>();

                var query = tableInfo.GetSelectStatementForAllFields();
                tableInfo.AddWhereByIdClause(query);

                command.CommandText = query.ToString();
                command.CreateAndAddInputParameter(tableInfo.PrimaryKey.DbType, tableInfo.GetPrimaryKeyParameterName(), id);
                return Hydrater.HydrateEntity<TEntity>(command);
            }
        }

        public TEntity Insert(TEntity entity)
        {
            using (var command = CreateCommand())
            {
                var tableInfo = MetaDataStore.GetTableInfoFor<TEntity>();

                command.CommandText = tableInfo.GetInsertStatement();

                foreach (var parameterInfo in tableInfo.GetParametersForInsert(entity))
                {
                    command.CreateAndAddInputParameter(parameterInfo.DbType, parameterInfo.Name, parameterInfo.Value);
                }

                object id = Convert.ChangeType(command.ExecuteScalar(), tableInfo.PrimaryKey.DotNetType);
                tableInfo.PrimaryKey.PropertyInfo.SetValue(entity, id, null);
                this.IdentityMap.Store(typeof(TEntity), id, entity);
                return entity;
            }
        }

        public TEntity Update(TEntity entity)
        {
            using (var command = CreateCommand())
            {
                var tableInfo = MetaDataStore.GetTableInfoFor<TEntity>();

                command.CommandText = tableInfo.GetUpdateStatement();

                foreach (var parameterInfo in tableInfo.GetParametersForUpdate(entity))
                {
                    command.CreateAndAddInputParameter(parameterInfo.DbType, parameterInfo.Name, parameterInfo.Value);
                }

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Update failed, no rows updated");
                }
                return entity;
            }
        }

        public void Delete(TEntity entity)
        {
            using (var command = CreateCommand())
            {
                var tableInfo = MetaDataStore.GetTableInfoFor<TEntity>();
                command.CommandText = tableInfo.GetDeleteStatement();
                command.CreateAndAddInputParameter(tableInfo.PrimaryKey.DbType, tableInfo.GetPrimaryKeyParameterName(), tableInfo.GetPrimaryKeyParameterValue(entity));
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Delete failed, no rows deleted");
                }
            }
        }

        #endregion
    }
}