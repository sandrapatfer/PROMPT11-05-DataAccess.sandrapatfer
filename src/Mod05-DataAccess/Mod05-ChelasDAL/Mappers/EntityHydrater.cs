namespace Mod05_ChelasDAL.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    using Mod05_ChelasDAL.Metadata;
    using Castle.DynamicProxy;

    public class EntityHydrater
    {
        private readonly MetaDataStore metaDataStore;
        private readonly IdentityMap identityMap;

        public EntityHydrater(MetaDataStore metaDataStore, IdentityMap identityMap)
        {
            this.metaDataStore = metaDataStore;
            this.identityMap = identityMap;
        }

        public TEntity HydrateEntity<TEntity>(SqlCommand command) where TEntity : new()
        {
            IDictionary<string, object> values;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows) return default(TEntity);
                reader.Read();
                values = GetValuesFromCurrentRow(reader);
            }

            return CreateEntityFromValues<TEntity>(values);
        }

        public IEnumerable<TEntity> HydrateEntities<TEntity>(SqlCommand command) where TEntity : new()
        {
            var rows = new List<IDictionary<string, object>>();
            var entities = new List<TEntity>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add(GetValuesFromCurrentRow(reader));
                }
            }

            foreach (var row in rows)
            {
                entities.Add(CreateEntityFromValues<TEntity>(row));
            }

            return entities;
        }

        private IDictionary<string, object> GetValuesFromCurrentRow(SqlDataReader dataReader)
        {
            var values = new Dictionary<string, object>();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                values.Add(dataReader.GetName(i), dataReader.GetValue(i));
            }

            return values;
        }

        private TEntity CreateEntityFromValues<TEntity>(IDictionary<string, object> values) where TEntity: new()
        {
            var tableInfo = metaDataStore.GetTableInfoFor<TEntity>();

            var cachedEntity = this.identityMap.TryToFind(typeof(TEntity), values[tableInfo.PrimaryKey.Name]);
            if (cachedEntity != null) return (TEntity)cachedEntity;

            var entity = new TEntity();
            Hydrate(tableInfo, entity, values);
            
            this.identityMap.Store(typeof(TEntity), values[tableInfo.PrimaryKey.Name], entity);
            return entity;
        }

        private void Hydrate<TEntity>(TableInfo tableInfo, TEntity entity, IDictionary<string, object> values)
        {
            tableInfo.PrimaryKey.PropertyInfo.SetValue(entity, values[tableInfo.PrimaryKey.Name], null);
            SetRegularColumns(tableInfo, entity, values);
            SetReferenceProperties(tableInfo, entity, values);
        }

        private void SetRegularColumns<TEntity>(TableInfo tableInfo, TEntity entity, IDictionary<string, object> values)
        {
            foreach (var columnInfo in tableInfo.Columns)
            {
                if (columnInfo.PropertyInfo.CanWrite)
                {
                    object value = values[columnInfo.Name];
                    if (value is DBNull) value = null;
                    columnInfo.PropertyInfo.SetValue(entity, value, null);
                }
            }
        }

        private void SetReferenceProperties<TEntity>(TableInfo tableInfo, TEntity entity, IDictionary<string, object> values)
        {
            foreach (var referenceInfo in tableInfo.References)
            {
                if (referenceInfo.PropertyInfo.CanWrite)
                {
                    object foreignKeyValue = values[referenceInfo.Name];

                    if (foreignKeyValue is DBNull)
                    {
                        referenceInfo.PropertyInfo.SetValue(entity, null, null);
                    }
                    else
                    {
                        var referencedEntity = this.identityMap.TryToFind(referenceInfo.ReferenceType, foreignKeyValue) ??
                                               CreateProxy(tableInfo, referenceInfo, foreignKeyValue);

                        referenceInfo.PropertyInfo.SetValue(entity, referencedEntity, null);
                    }
                }
            }
        }

        private object CreateProxy(TableInfo tableInfo, ReferenceInfo referenceInfo, object foreignKeyValue)
        {
            ProxyGenerator generator = new ProxyGenerator();
            return generator.CreateClassProxy(referenceInfo.ReferenceType, new ReferenceInterceptor(referenceInfo, foreignKeyValue));
        }


    }

    public class ReferenceInterceptor : StandardInterceptor
    {
        ReferenceInfo _referenceInfo;
        object _foreignKeyValue;

        public ReferenceInterceptor(ReferenceInfo referenceInfo, object foreignKeyValue)
        {
            _referenceInfo = referenceInfo;
            _foreignKeyValue = foreignKeyValue;
        }

        public override void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.Equals(_referenceInfo.Name))
            {

            }
            base.Intercept(invocation);
        }
    }
}